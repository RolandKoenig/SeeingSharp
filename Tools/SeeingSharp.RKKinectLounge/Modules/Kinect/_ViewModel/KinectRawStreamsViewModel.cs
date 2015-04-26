using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    public class KinectRawStreamsViewModel : NavigateableViewModelBase
    {
        private SceneViewModel m_currentScene;
        private KinectRawStreamPresenter m_rawStreamProcessor;
        private List<Scene> m_availableScenes;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectRawStreamsViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent of this ViewModel object.</param>
        public KinectRawStreamsViewModel(NavigateableViewModelBase parent)
            : base(parent)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            m_rawStreamProcessor =
                SeeingSharpApplication.Current.GetSingleton<KinectRawStreamPresenter>();
            m_availableScenes = m_rawStreamProcessor.GetRawStreamPresenterScenes().ToList();
            m_currentScene = new SceneViewModel(m_availableScenes[0]);

            this.CommandMovePageLeft = new DelegateCommand(() =>
                {
                    int currentIndex = m_availableScenes.IndexOf(m_currentScene.Scene);
                    
                    currentIndex--;
                    if (currentIndex < 0) { currentIndex = m_availableScenes.Count - 1; }

                    this.CurrentScene = new SceneViewModel(m_availableScenes[currentIndex]);
                });
            this.CommandMovePageRight = new DelegateCommand(() =>
                {
                    int currentIndex = m_availableScenes.IndexOf(m_currentScene.Scene);

                    currentIndex++;
                    if (currentIndex >= m_availableScenes.Count) { currentIndex = 0; }

                    this.CurrentScene = new SceneViewModel(m_availableScenes[currentIndex]);
                });
        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// This method is used when we are navigating backward.. =&gt; We don't reuse old ViewModels, instead, we
        /// reload them completely.
        /// </summary>
        public override NavigateableViewModelBase CreateNewWithSameTarget()
        {
            return new KinectRawStreamsViewModel(this.ParentViewModel);
        }

        /// <summary>
        /// Loads the preview content internal asynchronous.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        protected override async Task LoadPreviewContentInternalAsync(CancellationToken cancelToken)
        {
            if (m_availableScenes == null) { return; }

            foreach (var actScene in m_availableScenes)
            {
                base.ThumbnailViewModels.Add(new SceneViewModel(actScene));
            }

            await Task.Delay(100);
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

        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Raw Frames";
            }
        }

        public DelegateCommand CommandMovePageLeft
        {
            get;
            private set;
        }

        public DelegateCommand CommandMovePageRight
        {
            get;
            private set;
        }

        public SceneViewModel CurrentScene
        {
            get { return m_currentScene; }
            set
            {
                if(m_currentScene != value)
                {
                    m_currentScene = value;
                    RaisePropertyChanged(() => CurrentScene);
                }
            }
        }
    }
}
