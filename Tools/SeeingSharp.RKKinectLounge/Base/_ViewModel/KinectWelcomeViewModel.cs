#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion License information (SeeingSharp and all based games/applications)

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
using Microsoft.Kinect.Input;
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;

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
            if (Directory.Exists(sourceDirectory))
            {
                foreach (string actFileName in Directory.GetFiles(sourceDirectory))
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
                    this.WelcomeScreenImages.Add(new Image() { Source = actBitmapImage, Stretch = Stretch.UniformToFill });
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