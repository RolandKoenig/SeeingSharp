#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
        #region Main references for Direct2D
        private D2D.RenderTarget m_renderTarget;
        private D2D.Device m_deviceD2D;
        private D2D.DeviceContext m_deviceContextD2D;
        #endregion

        #region Members for fallback method on older windows platforms
        private D3D11.Texture2D m_dummyRenderTargetTexture;
        private Direct2DOverlayRenderer m_dummyDirect2DOverlay;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerD3D11" /> class.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="engineDevice">The engine device.</param>
        internal DeviceHandlerD2D(GraphicsCore core, EngineDevice engineDevice)
        {
            bool doFallbackMethod = core.Force2DFallbackMethod || (core.FactoryD2D_2 == null);

            // Do default method (Windows 8 and newer)
            if (!doFallbackMethod)
            {
                try
                {
                    using (DXGI.Device dxgiDevice = engineDevice.DeviceD3D11.QueryInterface<DXGI.Device>())
                    {
                        m_deviceD2D = new D2D.Device1(engineDevice.Core.FactoryD2D_2, dxgiDevice);
                        m_deviceContextD2D = new SharpDX.Direct2D1.DeviceContext(
                            m_deviceD2D,
                            D2D.DeviceContextOptions.None);
                        m_renderTarget = m_deviceContextD2D;
                    }
                }
                catch (Exception) { doFallbackMethod = true; }
            }

            // Fallback method (on older windows platforms (< Windows 8))
            if (doFallbackMethod)
            {
                m_renderTarget = null;
                m_deviceD2D = null;
                m_deviceContextD2D = null;

                m_dummyRenderTargetTexture = GraphicsHelper.CreateRenderTargetTextureDummy(
                    engineDevice.DeviceD3D11, 32, 32);
                m_dummyDirect2DOverlay = new Direct2DOverlayRenderer(
                    engineDevice, m_dummyRenderTargetTexture, 32, 32, DpiScaling.Default);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return
                    (m_renderTarget != null) ||
                    ((m_dummyDirect2DOverlay != null) && (m_dummyDirect2DOverlay.IsLoaded));
            }
        }
    
        /// <summary>
        /// Gets a reference to the Direct2D view to the device.
        /// </summary>
        public D2D.Device Device
        {
            get { return m_deviceD2D; }
        }

        /// <summary>
        /// Gets a reference to the device DeviceContext for rendering.
        /// </summary>
        public D2D.DeviceContext DeviceContext
        {
            get { return m_deviceContextD2D; }
        }

        internal D2D.RenderTarget RenderTarget
        {
            get
            {
                if (m_renderTarget != null) { return m_renderTarget; }
                else if(m_dummyDirect2DOverlay != null) { return m_dummyDirect2DOverlay.RenderTarget2D; }
                else { return null; }
            }
        }

        public bool IsUsingFallbackMethod
        {
            get { return m_dummyDirect2DOverlay != null; }
        }
    }
}