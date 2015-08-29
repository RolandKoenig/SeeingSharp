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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Core
{
    public class DeviceHandlerD2D
    {
        // Main references for Direct2D
        private D2D.RenderTarget m_renderTarget;
#if UNIVERSAL
        private D2D.Device1 m_deviceD2D;
        private D2D.DeviceContext1 m_deviceContextD2D;
#else
        private D3D11.Texture2D m_dummyRenderTargetTexture;
        private Direct2DOverlayRenderer m_dummyDirect2DOverlay;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerD3D11" /> class.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="engineDevice">The engine device.</param>
        internal DeviceHandlerD2D(GraphicsCore core, EngineDevice engineDevice)
        {
#if UNIVERSAL
            using (DXGI.Device dxgiDevice = engineDevice.DeviceD3D11.QueryInterface<DXGI.Device>())
            {
                m_deviceD2D = new D2D.Device1(engineDevice.Core.FactoryD2D, dxgiDevice);
                m_deviceContextD2D = new SharpDX.Direct2D1.DeviceContext1(
                    m_deviceD2D,
                    D2D.DeviceContextOptions.None);
                m_renderTarget = m_deviceContextD2D;
            }
#else
            m_dummyRenderTargetTexture = GraphicsHelper.CreateRenderTargetTextureDummy(
                engineDevice.DeviceD3D11, 32, 32);
            m_dummyDirect2DOverlay = new Direct2DOverlayRenderer(engineDevice, m_dummyRenderTargetTexture, 32, 32, DpiScaling.Default);
            m_renderTarget = m_dummyDirect2DOverlay.IsLoaded ? m_dummyDirect2DOverlay.RenderTarget2D : null;
#endif
        }

        public bool IsLoaded
        {
            get
            {
                return m_renderTarget != null;
            }
        }

#if UNIVERSAL        
        /// <summary>
        /// Gets a reference to the Direct2D view to the device.
        /// </summary>
        public D2D.Device1 Device
        {
            get { return m_deviceD2D; }
        }

        /// <summary>
        /// Gets a reference to the device DeviceContext for rendering.
        /// </summary>
        public D2D.DeviceContext1 DeviceContext
        {
            get { return m_deviceContextD2D; }
        }
#endif

        internal D2D.RenderTarget RenderTarget
        {
            get
            {
                return m_renderTarget;
            }
        }
    }
}