using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FrozenSky.RKKinectLounge.Base
{
    public class AsyncLoadingViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLoadingViewModel"/> class.
        /// </summary>
        /// <param name="loadingTask">The loading task.</param>
        public AsyncLoadingViewModel(Task loadingTask)
        {
            if (Dispatcher.CurrentDispatcher == null) { throw new FrozenSkyException("Unable to create a AsyncLoadingViewModel if we are currently not on the UI thread!"); }
            if (loadingTask == null) { throw new ArgumentNullException("loadingTask"); }

            this.IsLoadingScreenVisible = false;

            loadingTask.ContinueWith(
                (task) =>
                {
                    this.IsLoadingScreenVisible = true;
                    base.RaisePropertyChanged(() => this.IsLoadingScreenVisible);
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Tells the system whether we are loading content currently.
        /// </summary>
        public bool IsLoadingScreenVisible
        {
            get;
            set;
        }
    }
}
