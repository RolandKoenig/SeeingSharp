using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// Define assembly attributes for the type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.RKKinectLounge.Modules.Multimedia.MultimediaModule),
    contractType: typeof(SeeingSharp.RKKinectLounge.Base.IKinectLoungeModule))]

namespace SeeingSharp.RKKinectLounge.Modules.Multimedia
{
    public class MultimediaModule : IKinectLoungeModule
    {
        /// <summary>
        /// Gets a collection containing all global ResourceDictionaries.
        /// These ones can be used to register generic Styles, DataTemplates, ...
        /// </summary>
        public IEnumerable<ResourceDictionary> GetGlobalResourceDictionaries()
        {
            yield return new ResourceDictionary()
            {
                Source = new Uri("Modules/Multimedia/_Resources/GlobalResources.xaml", UriKind.Relative)
            };
        }

        /// <summary>
        /// Creates the view object for the given ViewModel.
        /// The created object will be displayed on the whole window.
        /// </summary>
        /// <param name="viewModel">The view model for which to create a view.</param>
        public FrameworkElement TryCreateFullView(NavigateableViewModelBase viewModel)
        {
            FolderViewModel actFolder = viewModel as FolderViewModel;
            if (actFolder == null) { return null; }

            MultimediaFolderViewModelExtension actFolderExt = actFolder.TryGetExtension<MultimediaFolderViewModelExtension>();
            if (actFolderExt == null) { return null; }

            // Choose by ViewType
            switch (actFolder.FolderConfig.ViewType)
            {
                case Constants.VIEW_TYPE_IMAGE_SLIDER:
                    return new MultimediaSliderView();
            }

            // Automatic detection when no ViewType given
            if ((string.IsNullOrEmpty(actFolder.FolderConfig.ViewType)) &&
                (actFolderExt.CountFullImages > 0) &&
                (actFolder.SubViewModels.Count == 0))
            {
                if (actFolderExt.CountFullImages > 1) { return new MultimediaSliderView(); }
                else { return new MultimediaSingleView(); }
            }

            return null;
        }
    }
}
