using FrozenSky.RKKinectLounge.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectDashboardViewModel : NavigateableViewModelBase
    {
        public KinectDashboardViewModel(NavigateableViewModelBase parent)
            : base(parent)
        {

        }

        protected override Task LoadPreviewContentInternalAsync(CancellationToken cancelToken)
        {
            return Task.Delay(100);
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

        public override string DisplayName
        {
            get { return "Dashboard"; }
        }
    }
}
