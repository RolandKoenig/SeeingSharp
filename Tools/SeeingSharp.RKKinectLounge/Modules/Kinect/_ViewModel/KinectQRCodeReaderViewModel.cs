using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    public class KinectQRCodeReaderViewModel : NavigateableViewModelBase
    {
        private SceneViewModel m_currentScene;
        private KinectQRCodePresenter m_rawStreamProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectQRCodeReaderViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent of this ViewModel object.</param>
        public KinectQRCodeReaderViewModel(NavigateableViewModelBase parent)
            : base(parent)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            m_rawStreamProcessor =
                SeeingSharpApplication.Current.GetSingleton<KinectQRCodePresenter>();
            m_currentScene = new SceneViewModel(m_rawStreamProcessor.Scene);
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
            if (m_currentScene == null) { return; }

            await Task.Delay(100);

            base.ThumbnailViewModels.Add(m_currentScene);

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
                return "QR-Code Reader";
            }
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
