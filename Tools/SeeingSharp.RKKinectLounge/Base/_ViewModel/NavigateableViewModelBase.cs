#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using System.Collections.Specialized;

namespace SeeingSharp.RKKinectLounge.Base
{
    public abstract class NavigateableViewModelBase : ViewModelBase
    {
        #region Tasks for content loading
        private NavigateableViewModelBase m_parentFolder;
        private Task m_loadPreviewContentTask;
        private Task m_loadDetailContentTask;
        #endregion

        #region Variables for navigation and content
        private List<INavigateableViewModelExtension> m_vmExtensions;
        private ObservableCollection<NavigateableViewModelBase> m_subViewModels;
        private ObservableCollection<object> m_thumbnailViewModels;
        private bool m_hasBigSizeChildren;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateableViewModelBase"/> class.
        /// </summary>
        /// <param name="parent">The parent of this ViewModel object.</param>
        public NavigateableViewModelBase(NavigateableViewModelBase parent)
        {
            m_hasBigSizeChildren = true;

            m_parentFolder = parent;
            m_subViewModels = new ObservableCollection<NavigateableViewModelBase>();
            m_subViewModels.CollectionChanged += OnSubViewModels_CollectionChanged;
            m_thumbnailViewModels = new ObservableCollection<object>();

            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Get all ViewModel extensions
            m_vmExtensions = SeeingSharpApplication.Current.TypeQuery.GetAndInstanciateByContract<INavigateableViewModelExtension>();
            for (int loop = 0; loop < m_vmExtensions.Count; loop++)
            {
                if (!m_vmExtensions[loop].ExtendsViewModelType(this))
                {
                    m_vmExtensions.RemoveAt(loop);
                    loop--;
                }
            }
        }

        /// <summary>
        /// Gets the ViewModel extension of the given type.
        /// </summary>
        public T TryGetExtension<T>()
            where T : class, INavigateableViewModelExtension
        {
            Type extensionType = typeof(T);
            foreach (var actExtension in m_vmExtensions)
            {
                if (actExtension.GetType() == extensionType) { return actExtension as T; }
            }
            return null;
        }

        /// <summary>
        /// Gets the extension with the given type.
        /// If it does not exist, then create and register it.
        /// </summary>
        /// <typeparam name="T">The type of the extension.</typeparam>
        internal T GetAndEnsureExtension<T>()
           where T : class, INavigateableViewModelExtension, new()
        {
            // Try to get an existing object.
            T result = TryGetExtension<T>();
            if (result != null) { return result; }

            // Create the extension object
            result = new T();
            if (!result.ExtendsViewModelType(this))
            {
                throw new ApplicationException(string.Format(
                    "Invalid extension of type {0} for this ViewModel of type {1}!",
                    typeof(T).FullName, this.GetType().FullName));
            }
            m_vmExtensions.Add(result);

            return result;
        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// This method is used when we are navigating backward.. => We don't reuse old ViewModels, instead, we
        /// reload them completely.
        /// </summary>
        public virtual NavigateableViewModelBase CreateNewWithSameTarget()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Triggers loading of the inner configuration.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        public async Task LoadPreviewContentAsync(CancellationToken cancelToken)
        {
            if (m_loadPreviewContentTask == null)
            {
                // Load content defined by current type
                m_loadPreviewContentTask = LoadPreviewContentInternalAsync(cancelToken);
                await m_loadPreviewContentTask;

                // Load content on all extensions
                foreach (INavigateableViewModelExtension actExtension in m_vmExtensions)
                {
                    await actExtension.LoadPreviewContentAsync(this, cancelToken);
                }
            }
        }

        // <summary>
        /// Triggers loading of the inner configuration.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        protected abstract Task LoadPreviewContentInternalAsync(CancellationToken cancelToken);

        /// <summary>
        /// Triggers loading of all inner contents.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        public async Task LoadDetailContentAsync(CancellationToken cancelToken)
        {
            if (m_loadDetailContentTask == null)
            {
                m_loadDetailContentTask = LoadDetailContentInternalAsync(cancelToken);
                await m_loadDetailContentTask;

                // Load content on all extensions
                foreach (INavigateableViewModelExtension actExtension in m_vmExtensions)
                {
                    await actExtension.LoadDetailContentAsync(this, cancelToken);
                }
            }
        }

        /// <summary>
        /// Triggers loading of all inner contents.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        protected virtual async Task LoadDetailContentInternalAsync(CancellationToken cancelToken)
        {
            // Load content on all extensions
            foreach (INavigateableViewModelExtension actExtension in m_vmExtensions)
            {
                await actExtension.LoadDetailContentAsync(this, cancelToken);
            }
        }

        /// <summary>
        /// Unloads all inner content which was loaded.
        /// </summary>
        public async Task UnloadAsync()
        {
            // Unload detail contents (if loaded before)
            if (m_loadDetailContentTask != null)
            {
                await m_loadDetailContentTask;
                this.UnloadDetailContentInternal();
            }

            // Unload preview contents (if loaded before)
            if (m_loadPreviewContentTask != null)
            {
                await m_loadPreviewContentTask;
                this.UnloadPreviewContentInternal();
            }

            // Clear subfolder collection finally
            this.SubViewModels.Clear();
        }

        /// <summary>
        /// Unloads all inner contents.
        /// </summary>
        protected abstract void UnloadDetailContentInternal();

        /// <summary>
        /// Unloads all inner configuration.
        /// </summary>
        protected abstract void UnloadPreviewContentInternal();

        private void OnSubViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(m_subViewModels.Count > Constants.MAX_SUBITEM_COUNT_FOR_BIG_TILES)
            {
                this.HasBigSizeChildren = false;
            }
        }

        /// <summary>
        /// Gets the parent ViewModel.
        /// </summary>
        public NavigateableViewModelBase ParentViewModel
        {
            get { return m_parentFolder; }
        }

        /// <summary>
        /// Gets the <see cref="INavigateableViewModelExtension"/> with the specified extension short name.
        /// An exception is raised, if there is no corresponding extension available.
        /// </summary>
        public INavigateableViewModelExtension this[string extensionShortName]
        {
            get
            {
                return m_vmExtensions
                    .Where((actExt) => actExt.ShortName.Equals(extensionShortName, StringComparison.InvariantCultureIgnoreCase))
                    .First();
            }
        }

        /// <summary>
        /// Gets a collection containing all subviewmodels (alls subfolders for navigation).
        /// </summary>
        public ObservableCollection<NavigateableViewModelBase> SubViewModels
        {
            get { return m_subViewModels; }
        }

        /// <summary>
        /// Gets the a collection containing all thumbnails to be displayed within tiles.
        /// </summary>
        public ObservableCollection<object> ThumbnailViewModels
        {
            get { return m_thumbnailViewModels; }
        }

        public virtual string DisplayName
        {
            get { return "(NoName)"; }
        }

        public bool HasBigSizeChildren
        {
            get { return m_hasBigSizeChildren; }
            set
            {
                if(m_hasBigSizeChildren != value)
                {
                    m_hasBigSizeChildren = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}