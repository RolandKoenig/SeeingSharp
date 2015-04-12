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

using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D2D = SharpDX.Direct2D1;

namespace FrozenSky.Multimedia.Drawing2D
{
    public class SolidBrushResource : BrushResource
    {
        private D2D.SolidColorBrush[] m_loadedBrushes;
        private Color4 m_singleColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrushResource" /> class.
        /// </summary>
        /// <param name="singleColor">Color of the single.</param>
        public SolidBrushResource(Color4 singleColor)
        {
            m_loadedBrushes = new D2D.SolidColorBrush[GraphicsCore.Current.DeviceCount];

            m_singleColor = singleColor;
        }

        /// <summary>
        /// Unloads all resources loaded on the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to unload the resource.</param>
        internal override void UnloadResources(EngineDevice engineDevice)
        {
            D2D.Brush brush = m_loadedBrushes[engineDevice.DeviceIndex];
            if(brush != null)
            {
                GraphicsHelper.DisposeObject(brush);
                m_loadedBrushes[engineDevice.DeviceIndex] = null;
            }
        }

        /// <summary>
        /// Gets the brush for the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to get the brush.</param>
        internal override D2D.Brush GetBrush(EngineDevice engineDevice)
        {
            // Check for disposed state
            if (base.IsDisposed) { throw new ObjectDisposedException(this.GetType().Name); }

            D2D.SolidColorBrush result = m_loadedBrushes[engineDevice.DeviceIndex];
            if (result == null)
            {
                // Load the brush
                result = new D2D.SolidColorBrush(engineDevice.FakeRenderTarget2D, m_singleColor.ToDXColor());
                m_loadedBrushes[engineDevice.DeviceIndex] = result;
            }

            return result;
        }
    }
}