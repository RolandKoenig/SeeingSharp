using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Define assembly attributes for the type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.RKKinectLounge.Modules.Kinect.KinectFolderViewModelExtension),
    contractType: typeof(SeeingSharp.RKKinectLounge.Base.INavigateableViewModelExtension))]

namespace SeeingSharp.RKKinectLounge.Modules.Kinect
{
    public class KinectFolderViewModelExtension : INavigateableViewModelExtension
    {
        /// <summary>
        /// Does this object extend the given object?
        /// Return false to ensure that no further logic is executed here.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        public bool ExtendsViewModelType(NavigateableViewModelBase ownerViewModel)
        {
            return ownerViewModel is MainFolderViewModel;
        }

        /// <summary>
        /// Loads the preview content asynchronous.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public Task LoadPreviewContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken)
        {
            return Task.Delay(100);
        }

        public async Task LoadDetailContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken)
        {
            // Load the ViewModel
            KinectRootViewModel rootViewModel = new KinectRootViewModel(ownerViewModel);
            await rootViewModel.LoadPreviewContentAsync(cancelToken);

            // Register the ViewModel on the owner
            ownerViewModel.SubViewModels.Add(rootViewModel);
        }

        public void Unload()
        {
            
        }

        public string ShortName
        {
            get { return "KinectRoot"; }
        }
    }
}
