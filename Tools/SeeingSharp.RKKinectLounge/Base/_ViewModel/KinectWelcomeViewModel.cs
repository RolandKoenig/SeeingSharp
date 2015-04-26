using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using Microsoft.Kinect.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class KinectWelcomeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KinectWelcomeViewModel"/> class.
        /// </summary>
        public KinectWelcomeViewModel()
        {
            // Set all welcome screens
            this.WelcomeScreenImages = new ObservableCollection<object>();

            if (!SeeingSharpApplication.IsInitialized) { return; }

            this.CommandManualEnter = new DelegateCommand(() => 
                SeeingSharpApplication.Current.UIMessenger.Publish<MessageManualEnter>());

            // Load all images for the WelcomeScreen
            string sourceDirectory =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Assets\\WelcomeScreens");
            if(Directory.Exists(sourceDirectory))
            {
                foreach(string actFileName in Directory.GetFiles(sourceDirectory))
                {
                    // Do only load supported formats
                    if (Array.IndexOf(Constants.SUPPORTED_IMAGE_FORMATS, Path.GetExtension(actFileName)) < 0)
                    {
                        continue;
                    }

                    // Load a reference to the bitmap
                    BitmapImage actBitmapImage = new BitmapImage();
                    actBitmapImage.BeginInit();
                    actBitmapImage.UriSource = new Uri(actFileName, UriKind.Absolute);
                    actBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    actBitmapImage.EndInit();

                    // Append bitmap to WelcomeScreen collection
                    this.WelcomeScreenImages.Add(new Image() { Source = actBitmapImage, Stretch=Stretch.UniformToFill });
                }
            }
        }

        public ObservableCollection<object> WelcomeScreenImages
        {
            get;
            private set;
        }

        public DelegateCommand CommandManualEnter
        {
            get;
            private set;
        }
    }
}
