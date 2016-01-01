#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Samples.Base;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UniversalWindowsSampleContainer
{
    public class SampleViewModel : ViewModelBase
    {
        private SampleDescription m_sampleDesc;
        private BitmapImage m_bitmap;
        private bool m_bitmapLoadTriggered;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleViewModel"/> class.
        /// </summary>
        /// <param name="sampleDesc">The sample description.</param>
        public SampleViewModel(SampleDescription sampleDesc)
        {
            m_sampleDesc = sampleDesc;
        }

        /// <summary>
        /// Loads the preview bitmap.
        /// </summary>
        private async void LoadBitmap()
        {
            BitmapImage newImage = new BitmapImage();
            using (Stream inStream = await m_sampleDesc.ImageLink.OpenInputStreamAsync())
            {
                newImage.DecodePixelHeight = 64;
                newImage.DecodePixelWidth = 64;
                newImage.DecodePixelType = DecodePixelType.Logical;
                await newImage.SetSourceAsync(inStream.AsRandomAccessStream());
            }
            this.Bitmap = newImage;
        }

        public string Name
        {
            get { return m_sampleDesc.Name; }
        }

        public string Category
        {
            get { return m_sampleDesc.Category; }
        }

        public BitmapImage Bitmap
        {
            get
            {
                if (!m_bitmapLoadTriggered) 
                {
                    m_bitmapLoadTriggered = true;
                    LoadBitmap(); 
                }

                return m_bitmap;
            }
            private set
            {
                if(m_bitmap != value)
                {
                    m_bitmap = value;
                    RaisePropertyChanged();
                }
            }
        }

        public SampleDescription SampleDescription
        {
            get { return m_sampleDesc; }
        }
    }
}
