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
#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;

// Namespace mappings
using MF = SharpDX.MediaFoundation;

namespace SeeingSharp.Multimedia.DrawingVideo
{
    public class CaptureDeviceInfo : IDisposable, ICheckDisposed
    {
        internal CaptureDeviceInfo(MF.Activate deviceSourceActivate)
        {
            deviceSourceActivate.EnsureNotNullOrDisposed("deviceSourceActivate");

            this.DeviceSourceActivate = deviceSourceActivate;
            this.FriendlyName = deviceSourceActivate.Get(
                MF.CaptureDeviceAttributeKeys.FriendlyName);
        }

        public override string ToString()
        {
            return this.FriendlyName;
        }

        /// <summary>
        /// Creates a media source pointing to this CaptureDevice.
        /// </summary>
        internal MF.MediaSource CreateMediaSource()
        {
            // Create the MediaSource object by given capture device
            IntPtr mediaSourcePtr = IntPtr.Zero;
            this.DeviceSourceActivate.ActivateObject(
                new Guid("279a808d-aec7-40c8-9c6b-a6b492c78a66"),
                out mediaSourcePtr);
            return new MF.MediaSource(mediaSourcePtr);
        }

        public void Dispose()
        {
            this.DeviceSourceActivate =
                GraphicsHelper.DisposeObject(this.DeviceSourceActivate);
        }

        internal MF.Activate DeviceSourceActivate
        {
            get;
            private set;
        }

        public string FriendlyName
        {
            get;
            private set;
        }

        public bool IsDisposed
        {
            get { return this.DeviceSourceActivate == null; }
        }
    }
}
#endif