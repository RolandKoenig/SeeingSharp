using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.RKKinectLounge.Modules.Kinect;
using SeeingSharp.RKKinectLounge.Modules.Multimedia;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class ViewFactory
    {
        /// <summary>
        /// Creates the view object for the given ViewModel.
        /// The created object will be displayed on the whole window.
        /// </summary>
        /// <param name="viewModel">The view model for which to create a view.</param>
        public static FrameworkElement CreateFullView(NavigateableViewModelBase viewModel)
        {
            // Try to create the view using one of the loaded modules
            // (first one wins)
            foreach(IKinectLoungeModule actModule in ModuleManager.LoadedModules)
            {
                FrameworkElement actResult = actModule.TryCreateFullView(viewModel);
                if (actResult != null) { return actResult; }
            }

            // Create a FolderView object if nothing else found
            FolderView result = new FolderView();
            result.DataContext = viewModel;
            return result;
        }
    }
}
