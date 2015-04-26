#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Core
{
    public class DeviceHandlerD2D
    {
        // Main references for Direct2D
        private D3D11.Texture2D m_dummyRenderTargetTexture;
        private Direct2DOverlayRenderer m_dummyDirect2DOverlay;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerD3D11" /> class.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="engineDevice">The engine device.</param>
        internal DeviceHandlerD2D(GraphicsCore core, EngineDevice engineDevice)
        {
            m_dummyRenderTargetTexture = GraphicsHelper.CreateRenderTargetTextureDummy(
                engineDevice.DeviceD3D11, 32, 32);
            m_dummyDirect2DOverlay = new Direct2DOverlayRenderer(engineDevice, m_dummyRenderTargetTexture, 32, 32, DpiScaling.Default);
        }

        public bool IsLoaded
        {
            get { return m_dummyDirect2DOverlay.IsLoaded; }
        }

        internal D2D.RenderTarget RenderTarget
        {
            get { return m_dummyDirect2DOverlay.RenderTarget2D; }
        }
    }
}