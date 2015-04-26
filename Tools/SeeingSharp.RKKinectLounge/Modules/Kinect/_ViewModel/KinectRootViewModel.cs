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
    public class KinectRootViewModel : NavigateableViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KinectRootViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent of this ViewModel object.</param>
        public KinectRootViewModel(NavigateableViewModelBase parent)
            : base(parent)
        {
            this.SubViewModels.Add(new KinectRawStreamsViewModel(this));
            this.SubViewModels.Add(new KinectDashboardViewModel(this));
            this.SubViewModels.Add(new KinectQRCodeReaderViewModel(this));
        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// This method is used when we are navigating backward.. =&gt; We don't reuse old ViewModels, instead, we
        /// reload them completely.
        /// </summary>
        public override NavigateableViewModelBase CreateNewWithSameTarget()
        {
            return new KinectRootViewModel(base.ParentViewModel);
        }

        /// <summary>
        /// Triggers loading of all inner contents.
        /// </summary>
        /// <param name="cancelToken">The cancellation token.</param>
        protected override async Task LoadDetailContentInternalAsync(CancellationToken cancelToken)
        {
            await base.LoadDetailContentInternalAsync(cancelToken);

            foreach(var actSubViewModel in this.SubViewModels)
            {
                await actSubViewModel.LoadPreviewContentAsync(cancelToken);
            }
        }

        /// <summary>
        /// Loads the preview content internal asynchronous.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        protected override async Task LoadPreviewContentInternalAsync(CancellationToken cancelToken)
        {
            KinectRawStreamPresenter rawStreamProcessor =
                SeeingSharpApplication.Current.GetSingleton<KinectRawStreamPresenter>();
            foreach (var actScene in rawStreamProcessor.GetRawStreamPresenterScenes(RawStreamSubset.OnlyColorAndDepth))
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
        /// Gets the Title of the KinectRoot page.
        /// </summary>
        public override string DisplayName
        {
            get { return "Kinect"; }
        }
    }
}
