#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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

//Some namespace mappings
using D3D10 = SharpDX.Direct3D10;
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;

namespace FrozenSky.Multimedia.Core
{
    // Overview Feature levels:
    //http://msdn.microsoft.com/en-us/library/windows/desktop/ff476876(v=vs.85).aspx

    // Informations on WARP
    //http://msdn.microsoft.com/en-us/library/windows/desktop/gg615082(v=vs.85).aspx#capabilities

    /// <summary>
    /// All initialization logic for the D3D10 device
    /// </summary>
    public class DeviceHandlerD3D10
    {
        //Resources from Direct3D10 api
        private D3D10.Device1 m_device;
        private D3D10.DeviceCreationFlags m_creationFlags;
        private D3D10.FeatureLevel m_featureLevel;
         
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerD3D11"/> class.
        /// </summary>
        /// <param name="dxgiAdapter">The corresponding DXGI adapter.</param>
        /// <param name="debugEnabled">Is debug enabled?</param>
        internal DeviceHandlerD3D10(TargetHardware targetHardware, DXGI.Adapter1 dxgiAdapter, bool debugEnabled)
        {
            //Define possible create flags
            D3D10.DeviceCreationFlags createFlagsBgra = D3D10.DeviceCreationFlags.BgraSupport;
            D3D10.DeviceCreationFlags createFlags = D3D10.DeviceCreationFlags.None;
            if (debugEnabled)
            {
                createFlagsBgra |= D3D10.DeviceCreationFlags.Debug;
                createFlags |= D3D10.DeviceCreationFlags.Debug;
            }

            //Define all steps on which we try to initialize Direct3D
            List<Tuple<D3D10.FeatureLevel, D3D10.DeviceCreationFlags>> initParameterQueue = 
                new List<Tuple<D3D10.FeatureLevel, D3D10.DeviceCreationFlags>>();
            if (targetHardware == TargetHardware.Minimalistic)
            {
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_9_1, createFlagsBgra));
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_9_1, createFlags));
            }
            else
            {
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_10_0, createFlagsBgra));
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_10_0, createFlags));
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_9_1, createFlagsBgra));
                initParameterQueue.Add(Tuple.Create(D3D10.FeatureLevel.Level_9_1, createFlags));
            }

            //Try to create the device, each defined configuration step by step
            foreach (Tuple<D3D10.FeatureLevel, D3D10.DeviceCreationFlags> actInitParameters in initParameterQueue)
            {
                D3D10.FeatureLevel featureLevel = actInitParameters.Item1;
                D3D10.DeviceCreationFlags direct3D11Flags = actInitParameters.Item2;

                try
                {
                    // Try to create the device
                    using (D3D10.Device device = new D3D10.Device1(dxgiAdapter, direct3D11Flags, featureLevel))
                    {
                        m_device = device.QueryInterface<D3D10.Device1>();
                    }

                    //Device successfully created, save all parameters and break this loop
                    m_featureLevel = featureLevel;
                    m_creationFlags = direct3D11Flags;
                    break;
                }
                catch { }
            }

            //Throw exception on failure
            if (m_device == null) { throw new FrozenSkyGraphicsException("Unable to initialize d3d11 device!"); }
        }

        /// <summary>
        /// Gets the Dxgi device object.
        /// </summary>
        internal DXGI.Device GetDxgiDevice()
        {
            if (m_device != null) { return new DXGI.Device(m_device.NativePointer); }
            else { return null; }
        }

        /// <summary>
        /// Gets current feature level.
        /// </summary>
        internal D3D10.FeatureLevel FeatureLevel
        {
            get { return m_featureLevel; }
        }

        /// <summary>
        /// Gets the Direct3D 11 device.
        /// </summary>
        internal D3D10.Device1 Device
        {
            get { return m_device; }
        }

        /// <summary>
        /// Gets the native pointer to the device object.
        /// </summary>
        public IntPtr DeviceNativePointer
        {
            get { return m_device.NativePointer; }
        }

        /// <summary>
        /// Is device successfully initialized?
        /// </summary>
        public bool IsInitialized
        {
            get { return m_device != null; }
        }

        /// <summary>
        /// Gets a short description containing info about the created device.
        /// </summary>
        public string DeviceModeDescription
        {
            get
            {
                if (m_device == null) { return "None"; }

                return m_featureLevel.ToString();
            }
        }

        /// <summary>
        /// Are Direct2D textures possible?
        /// </summary>
        public bool IsDirect2DTextureEnabled
        {
            get { return (m_creationFlags & D3D10.DeviceCreationFlags.BgraSupport) == D3D10.DeviceCreationFlags.BgraSupport; }
        }
    }
}
#endif