using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SeeingSharp.RKKinectLounge.Modules.Multimedia
{
    public class ImageViewModel : ViewModelBase
    {
        private ImageSource m_imageSource;
        private string m_filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewModel"/> class.
        /// </summary>
        public ImageViewModel()
        {
            m_filePath = string.Empty;
        }

        /// <summary>
        /// Preloads this iamge asynchronously using given desired width and height.
        /// </summary>
        internal async Task PreloadAsync(int desiredPixelWidth, int desiredPixelHeight)
        {
            BitmapImage actBitmapImage = null;

            await Task.Factory.StartNew(() =>
                {
                    actBitmapImage = new BitmapImage();
                    actBitmapImage.BeginInit();
                    actBitmapImage.UriSource = new Uri(m_filePath, UriKind.Absolute);
                    actBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    actBitmapImage.DecodePixelHeight = desiredPixelHeight;
                    actBitmapImage.EndInit();
                    actBitmapImage.Freeze();
                });

            this.ImageSoure = actBitmapImage;
        }

        public string FilePath
        {
            get { return m_filePath; }
            set { m_filePath = value; }
        }

        public ImageSource ImageSoure
        {
            get 
            { 
                return m_imageSource; 
            }
            internal set
            {
                if(m_imageSource != value)
                {
                    m_imageSource = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
