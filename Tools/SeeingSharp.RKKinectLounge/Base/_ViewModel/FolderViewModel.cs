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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class FolderViewModel : NavigateableViewModelBase
    {
        protected const double VIEW_WIDTH_MAIN = 1280;
        protected const double VIEW_HEIGHT_MAIN = 720;
        protected const double VIEW_WIDTH_THUMBNAILE = 400;
        protected const double VIEW_HEIGHT_THUMBNAILE = 225;

        private string m_basePath;
        private FolderConfiguration m_folderConfig;

        /// <summary>
        /// Prevents a default instance of the <see cref="FolderViewModel"/> class from being created.
        /// </summary>
        private FolderViewModel()
            : base(null)
        {
            m_basePath = string.Empty;
            m_folderConfig = FolderConfiguration.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderViewViewModell"/> class.
        /// </summary>
        /// <param name="parent">The parent viewmodel.</param>
        /// <param name="basePath">The base path this viewmodel is based on.</param>
        public FolderViewModel(NavigateableViewModelBase parent, string basePath)
            : base(parent)
        {
            m_basePath = basePath;

            // Set default folder configuration
            m_folderConfig = FolderConfiguration.Default;
        }

        /// <summary>
        /// Creates a dummy FolderViewModel object (e. g. for UnitTesting, DesignerData, ...).
        /// </summary>
        public static FolderViewModel CreateDummy()
        {
            return new FolderViewModel();
        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// This method is used when we are navigating backward.. =&gt; We don't reuse old ViewModels, instead, we
        /// reload them completely.
        /// </summary>
        public override NavigateableViewModelBase CreateNewWithSameTarget()
        {
            return new FolderViewModel(base.ParentViewModel, m_basePath);
        }

        /// <summary>
        /// Loads the inner configuration internal asynchronous.
        /// </summary>
        protected override async Task LoadPreviewContentInternalAsync(CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(m_basePath)) { return; }

            // Load configuration object
            string configFilePath = Path.Combine(m_basePath, Constants.BROWSING_FOLDER_CONFIG_FILE);
            if (File.Exists(configFilePath))
            {
                try
                {
                    m_folderConfig = await CommonTools.DeserializeFromXmlFile<FolderConfiguration>(configFilePath);
                }
                catch (Exception) { }
            }
            if (m_folderConfig == null) { m_folderConfig = FolderConfiguration.Default; }
        }

        /// <summary>
        /// Loads all folder contents.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        protected override async Task LoadDetailContentInternalAsync(CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(m_basePath)) { return; }

            // Load all subfolders folder-by-folder
            // Trigger loading of the description (image, displayname, ...) before coninuing with next one
            List<FolderViewModel> foundSubdirectories = new List<FolderViewModel>();
            foreach (string actSubdirectory in Directory.GetDirectories(m_basePath))
            {
                FolderViewModel actSubdirVM = new FolderViewModel(this, actSubdirectory);
                foundSubdirectories.Add(actSubdirVM);

                await actSubdirVM.LoadPreviewContentAsync(cancelToken);
                await Task.Delay(Constants.BROWSING_DELAY_TIME_PER_FOLDER_LOAD_MS);

                base.SubViewModels.Add(actSubdirVM);

                // Return here if cancellation is requested
                if (cancelToken.IsCancellationRequested) { return; }
            }
        }

        /// <summary>
        /// Unloads all inner contents.
        /// </summary>
        protected override void UnloadDetailContentInternal()
        {
        }

        /// <summary>
        /// Unloads all inner configuration.
        /// </summary>
        protected override void UnloadPreviewContentInternal()
        {
            m_folderConfig = null;
        }

        /// <summary>
        /// Gets the folder configuration.
        /// </summary>
        public FolderConfiguration FolderConfig
        {
            get { return m_folderConfig; }
        }

        public override string DisplayName
        {
            get { return Path.GetFileName(m_basePath); }
        }

        /// <summary>
        /// Gets the folder path that belongs to this ViewModel.
        /// </summary>
        public string BasePath
        {
            get
            {
                return m_basePath;
            }
        }
    }
}