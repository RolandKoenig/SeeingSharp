using FrozenSky.Infrastructure;
using FrozenSky.RKKinectLounge.Base;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// Define assembly attributes for the type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.RKKinectLounge.Modules.Kinect.KinectModule),
    contractType: typeof(FrozenSky.RKKinectLounge.Base.IKinectLoungeModule))]

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectModule : IKinectLoungeModule
    {
        /// <summary>
        /// Gets a collection containing all global ResourceDictionaries.
        /// These ones can be used to register generic Styles, DataTemplates, ...
        /// </summary>
        public IEnumerable<ResourceDictionary> GetGlobalResourceDictionaries()
        {
            yield return new ResourceDictionary()
            {
                Source = new Uri("Modules/Kinect/_Resources/GlobalResources.xaml", UriKind.Relative)
            };
        }

        /// <summary>
        /// Creates the view object for the given ViewModel.
        /// The created object will be displayed on the whole window.
        /// </summary>
        /// <param name="viewModel">The view model for which to create a view.</param>
        public FrameworkElement TryCreateFullView(NavigateableViewModelBase viewModel)
        {
            KinectDashboardViewModel dashboardViewModel = viewModel as KinectDashboardViewModel;
            if (dashboardViewModel != null) { return new KinectDashboardView(); }

            KinectRawStreamsViewModel rawStreamViewModel = viewModel as KinectRawStreamsViewModel;
            if (rawStreamViewModel != null) { return new KinectRawStreamsView(); }

            KinectQRCodeReaderViewModel qrViewModel = viewModel as KinectQRCodeReaderViewModel;
            if (qrViewModel != null) { return new KinectQRCodeReaderView(); }

            return null;
        }
    }
}
