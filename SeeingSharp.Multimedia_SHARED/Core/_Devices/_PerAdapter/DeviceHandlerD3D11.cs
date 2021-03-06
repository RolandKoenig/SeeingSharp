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
using System;
using System.Collections.Generic;
using SeeingSharp.Util;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;

namespace SeeingSharp.Multimedia.Core
{
    // Overview Feature levels:
    //http://msdn.microsoft.com/en-us/library/windows/desktop/ff476876(v=vs.85).aspx

    // Informations on WARP
    //http://msdn.microsoft.com/en-us/library/windows/desktop/gg615082(v=vs.85).aspx#capabilities

    /// <summary>
    /// All initialization logic for the D3D11 device
    /// </summary>
    public class DeviceHandlerD3D11
    {
        #region Resources from Direct3D11 api
        private DXGI.Adapter1 m_dxgiAdapter;
        private D3D11.Device1 m_device1;
        private D3D11.Device3 m_device3;
        private D3D11.DeviceContext m_immediateContext;
        private D3D11.DeviceContext3 m_immediateContext3;
        #endregion

        #region Parameters of created device
        private D3D11.DeviceCreationFlags m_creationFlags;
        private D3D.FeatureLevel m_featureLevel;
        private HardwareDriverLevel m_driverLevel;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHandlerD3D11"/> class.
        /// </summary>
        /// <param name="dxgiAdapter">The tasrget adapter.</param>
        /// <param name="debugEnabled">Is debug mode enabled?</param>
        internal DeviceHandlerD3D11(DXGI.Adapter1 dxgiAdapter, bool debugEnabled)
        {
            m_dxgiAdapter = dxgiAdapter;

            // Define possible create flags
            D3D11.DeviceCreationFlags createFlagsBgra = D3D11.DeviceCreationFlags.BgraSupport;
            D3D11.DeviceCreationFlags createFlags = D3D11.DeviceCreationFlags.None;
            if (debugEnabled)
            {
                createFlagsBgra |= D3D11.DeviceCreationFlags.Debug;
                createFlags |= D3D11.DeviceCreationFlags.Debug;
            }

            // Define all steps on which we try to initialize Direct3D
            List<Tuple<D3D.FeatureLevel, D3D11.DeviceCreationFlags, HardwareDriverLevel>> initParameterQueue =
                new List<Tuple<D3D.FeatureLevel, D3D11.DeviceCreationFlags, HardwareDriverLevel>>();

            // Define all trys for hardware initialization
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_11_1, createFlagsBgra, HardwareDriverLevel.Direct3D11));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_11_0, createFlagsBgra, HardwareDriverLevel.Direct3D11));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_10_0, createFlagsBgra, HardwareDriverLevel.Direct3D10));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_9_3, createFlagsBgra, HardwareDriverLevel.Direct3D9_3));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_9_2, createFlagsBgra, HardwareDriverLevel.Direct3D9_2));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_9_1, createFlagsBgra, HardwareDriverLevel.Direct3D9_1));
            initParameterQueue.Add(Tuple.Create(
                D3D.FeatureLevel.Level_10_0, createFlags, HardwareDriverLevel.Direct3D10));
            initParameterQueue.Add(Tuple.Create(
                 D3D.FeatureLevel.Level_9_3, createFlags, HardwareDriverLevel.Direct3D9_3));
            initParameterQueue.Add(Tuple.Create(
                 D3D.FeatureLevel.Level_9_2, createFlags, HardwareDriverLevel.Direct3D9_2));
            initParameterQueue.Add(Tuple.Create(
                 D3D.FeatureLevel.Level_9_1, createFlags, HardwareDriverLevel.Direct3D9_1));

            // Try to create the device, each defined configuration step by step
            foreach (Tuple<D3D.FeatureLevel, D3D11.DeviceCreationFlags, HardwareDriverLevel> actInitParameters in initParameterQueue)
            {
                D3D.FeatureLevel featureLevel = actInitParameters.Item1;
                D3D11.DeviceCreationFlags direct3D11Flags = actInitParameters.Item2;
                HardwareDriverLevel actDriverLevel = actInitParameters.Item3;

                try
                {
                    // Try to create the device using current parameters
                    using (D3D11.Device device = new D3D11.Device(dxgiAdapter, direct3D11Flags, featureLevel))
                    {
                        m_device1 = device.QueryInterface<D3D11.Device1>();
                        m_device3 = CommonTools.TryExecute(() => m_device1.QueryInterface<D3D11.Device3>());
                        if(m_device3 != null)
                        {
                            m_immediateContext3 = m_device3.ImmediateContext3;
                        }
                    }

                    // Device successfully created, save all parameters and break this loop
                    m_featureLevel = featureLevel;
                    m_creationFlags = direct3D11Flags;
                    m_driverLevel = actDriverLevel;
                    break;
                }
                catch (Exception) { }
            }

            // Throw exception on failure
            if (m_device1 == null) 
            { 
                throw new SeeingSharpGraphicsException("Unable to initialize d3d11 device!"); 
            }

            // Get immediate context from the device
            m_immediateContext = m_device1.ImmediateContext;
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        public void UnloadResources()
        {
            m_immediateContext = CommonTools.DisposeObject(m_immediateContext);
            m_immediateContext3 = CommonTools.DisposeObject(m_immediateContext3);
            m_device1 = CommonTools.DisposeObject(m_device1);
            m_device3 = CommonTools.DisposeObject(m_device3);

            m_creationFlags = D3D11.DeviceCreationFlags.None;
            m_featureLevel = D3D.FeatureLevel.Level_11_0;
        }

        /// <summary>
        /// Gets current feature level.
        /// </summary>
        internal D3D.FeatureLevel FeatureLevel
        {
            get { return m_featureLevel; }
        }

        /// <summary>
        /// Is the hardware Direct3D 10 or upper?
        /// </summary>
        public bool IsDirect3D10OrUpperHardware
        {
            get
            {
                return
                    (m_featureLevel == D3D.FeatureLevel.Level_10_0) ||
                    (m_featureLevel == D3D.FeatureLevel.Level_10_1) ||
                    (m_featureLevel == D3D.FeatureLevel.Level_11_0) ||
                    (m_featureLevel == D3D.FeatureLevel.Level_11_1);
            }
        }

        /// <summary>
        /// Is the hardware Direct3D 11 or upper?
        /// </summary>
        public bool IsDirect3D11OrUpperHardware
        {
            get
            {
                return
                    (m_featureLevel == D3D.FeatureLevel.Level_11_0) ||
                    (m_featureLevel == D3D.FeatureLevel.Level_11_1);
            }
        }

        /// <summary>
        /// Gets the Direct3D 11 device.
        /// </summary>
        internal D3D11.Device1 Device1
        {
            get { return m_device1; }
        }

        internal D3D11.Device3 Device3
        {
            get { return m_device3; }
        }

        /// <summary>
        /// Gets the native pointer to the device object.
        /// </summary>
        public IntPtr DeviceNativePointer
        {
            get { return m_device1.NativePointer; }
        }

        /// <summary>
        /// Gets the immediate context.
        /// </summary>
        internal D3D11.DeviceContext ImmediateContext
        {
            get { return m_immediateContext; }
        }

        /// <summary>
        /// Gets the immediate context.
        /// </summary>
        internal D3D11.DeviceContext3 ImmediateContext3
        {
            get { return m_immediateContext3; }
        }

        /// <summary>
        /// Is device successfully initialized?
        /// </summary>
        public bool IsInitialized
        {
            get { return m_device1 != null; }
        }

        /// <summary>
        /// Gets a short description containing info about the created device.
        /// </summary>
        public string DeviceModeDescription
        {
            get
            {
                if (m_device1 == null) { return "None"; }

                return m_dxgiAdapter.ToString() + " - " + m_featureLevel + (this.IsDirect2DTextureEnabled ? " - Bgra" : " - No Bgra");
            }
        }

        /// <summary>
        /// Gets the driver level.
        /// </summary>
        public HardwareDriverLevel DriverLevel
        {
            get { return m_driverLevel; }
        }

        /// <summary>
        /// Are Direct2D textures possible?
        /// </summary>
        public bool IsDirect2DTextureEnabled
        {
            get { return (m_creationFlags & D3D11.DeviceCreationFlags.BgraSupport) == D3D11.DeviceCreationFlags.BgraSupport; }
        }
    }
}
