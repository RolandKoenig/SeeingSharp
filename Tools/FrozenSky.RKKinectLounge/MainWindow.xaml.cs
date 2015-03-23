using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using Microsoft.Kinect.Toolkit.Input;
using FrozenSky.Infrastructure;

namespace FrozenSky.RKKinectLounge
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Use the default sensor
            if (!FrozenSkyApplication.IsInitialized) { return; }
            try
            {
                KinectRegion.SetKinectRegion(this, this.MainKinectRegion);

                // Get the current KinectSensor and attach it to the main KinectRegion object
                KinectSensor kinectSensor = KinectSensor.GetDefault();
                this.MainKinectRegion.KinectSensor = kinectSensor;

                // Load main EngagementManager when kinect is loaded
                kinectSensor.IsAvailableChanged += (sender, eArgs) =>
                {
                    if(kinectSensor.IsAvailable)
                    {
                        IKinectEngagementManager engagementManager = FrozenSkyApplication.Current.TryGetService<IKinectEngagementManager>();
                        if (engagementManager != null)
                        {
                            this.MainKinectRegion.SetKinectOnePersonManualEngagement(engagementManager);
                        }
                    }
                };
            }
            catch (Exception) { }
        }
    }
}
