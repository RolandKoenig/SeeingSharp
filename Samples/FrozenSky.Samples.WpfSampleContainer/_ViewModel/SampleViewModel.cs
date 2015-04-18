#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
#endregion
using FrozenSky.Samples.Base;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfSampleContainer
{
    public class SampleViewModel : ViewModelBase
    {
        private SampleDescription m_sampleDesc;
        private Lazy<BitmapImage> m_bitmap;

        public SampleViewModel(SampleDescription sampleDesc)
        {
            m_sampleDesc = sampleDesc;
            m_bitmap = new Lazy<BitmapImage>(() =>
            {
                BitmapImage newImage = new BitmapImage();
                newImage.CacheOption = BitmapCacheOption.OnLoad;

                using (Stream inStream = m_sampleDesc.ImageLink.OpenInputStream())
                {
                    newImage.BeginInit();
                    newImage.StreamSource = m_sampleDesc.ImageLink.OpenInputStream();
                    newImage.EndInit();
                }

                return newImage;
            });
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
                return m_bitmap.Value;
            }
        }

        public SampleDescription SampleDescription
        {
            get { return m_sampleDesc; }
        }
    }
}
