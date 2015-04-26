using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Base
{
    public interface INavigateableViewModelExtension
    {
        /// <summary>
        /// Does this object extend the given object?
        /// Return false to ensure that no further logic is executed here.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        bool ExtendsViewModelType(NavigateableViewModelBase ownerViewModel);

        /// <summary>
        /// Loads the preview content asynchronous.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        /// <param name="cancelToken">The cancel token.</param>
        Task LoadPreviewContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken);

        /// <summary>
        /// Loads all folder contents that belong to this extension.
        /// </summary>
        /// <param name="ownerViewModel">The ViewModel for which to load detail content.</param>
        /// <param name="cancelToken">The cancel token.</param>
        Task LoadDetailContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken);

        /// <summary>
        /// Unloads all contents which are loaded currently.
        /// </summary>
        void Unload();

        /// <summary>
        /// Gets the short name of this ViewModel extension.
        /// </summary>
        string ShortName { get; }
    }
}
