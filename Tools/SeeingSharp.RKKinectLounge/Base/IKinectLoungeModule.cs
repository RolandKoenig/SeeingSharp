using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeeingSharp.RKKinectLounge.Base
{
    public interface IKinectLoungeModule
    {
        /// <summary>
        /// Gets a collection containing all global ResourceDictionaries.
        /// These ones can be used to register generic Styles, DataTemplates, ...
        /// </summary>
        IEnumerable<ResourceDictionary> GetGlobalResourceDictionaries();

        /// <summary>
        /// Creates the view object for the given ViewModel.
        /// The created object will be displayed on the whole window.
        /// </summary>
        /// <param name="viewModel">The view model for which to create a view.</param>
        FrameworkElement TryCreateFullView(NavigateableViewModelBase viewModel);
    }
}
