﻿#region License information (SeeingSharp and all based games/applications)
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
using SeeingSharp.Util;

// Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Core
{
    public class DeviceHandlerDXGI
    {
        private DXGI.Factory2 m_factory;
        private DXGI.Adapter1 m_adapter;
#if UNIVERSAL
        private DXGI.Device3 m_device;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerDXGI"/> class.
        /// </summary>
        internal DeviceHandlerDXGI(DXGI.Adapter1 adapter, D3D11.Device device)
        {
#if UNIVERSAL
            m_device = device.QueryInterface<DXGI.Device3>();
#endif
            m_adapter = adapter;

            m_factory = m_adapter.GetParent<DXGI.Factory2>();
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        internal void UnloadResources()
        {
            m_factory = CommonTools.DisposeObject(m_factory);
            m_adapter = CommonTools.DisposeObject(m_adapter);
#if UNIVERSAL
            m_device = CommonTools.DisposeObject(m_device);
#endif
        }

        /// <summary>
        /// Gets current factory object.
        /// </summary>
        /// <value>The factory.</value>
        internal DXGI.Factory2 Factory
        {
            get { return m_factory; }
        }

#if UNIVERSAL
        /// <summary>
        /// Gets the DXGI device.
        /// </summary>
        internal DXGI.Device3 Device
        {
            get { return m_device; }
        }
#endif

        /// <summary>
        /// Gets current adapter used for drawing.
        /// </summary>
        internal DXGI.Adapter1 Adapter
        {
            get { return m_adapter; }
        }

        public bool IsInitialized
        {
            get
            {
                return (m_factory != null) &&
#if UNIVERSAL
                       (m_device != null) &&
#endif
                       (m_adapter != null);
            }
        }
    }
}
