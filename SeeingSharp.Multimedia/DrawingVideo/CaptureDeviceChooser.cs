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
#endregion
#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Core;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    public class CaptureDeviceChooser : IDisposable, ICheckDisposed
    {
        private CaptureDeviceInfo[] m_captureDeviceInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureDeviceChooser"/> class.
        /// </summary>
        public CaptureDeviceChooser()
        {
            MF.Activate[] mediaSourceActivates;
            using(MF.MediaAttributes attributes = new MF.MediaAttributes())
            {
                // Guid value taken from mfidl.h
                attributes.Set<Guid>(
                    MF.CaptureDeviceAttributeKeys.SourceType,
                    new Guid("8ac3587a-4ae7-42d8-99e0-0a6013eef90f"));

                // Query for all device
                mediaSourceActivates = MF.MediaFactory.EnumDeviceSources(attributes);
            }
            if (mediaSourceActivates == null) { mediaSourceActivates = new MF.Activate[0]; }

            // Create info objects
            m_captureDeviceInfos = new CaptureDeviceInfo[mediaSourceActivates.Length];
            for(int loop=0 ; loop<m_captureDeviceInfos.Length; loop++)
            {
                m_captureDeviceInfos[loop] = new CaptureDeviceInfo(mediaSourceActivates[loop]);
            }
        }

        public void Dispose()
        {
            if (m_captureDeviceInfos == null) { return; }

            foreach(var actDeviceInfo in m_captureDeviceInfos)
            {
                GraphicsHelper.DisposeObject(actDeviceInfo);
            }
            m_captureDeviceInfos = null;
        }

        public IEnumerable<CaptureDeviceInfo> DeviceInfos
        {
            get { return m_captureDeviceInfos; }
        }

        public bool IsDisposed
        {
            get { return m_captureDeviceInfos == null; }
        }
    }
}
#endif