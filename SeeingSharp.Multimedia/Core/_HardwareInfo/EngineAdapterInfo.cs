﻿#region License information (SeeingSharp and all based games/applications)
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
using System.Collections.ObjectModel;
using System.ComponentModel;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;

//Some type mappings
#if WINRT || UNIVERSAL
using DxgiFactory = SharpDX.DXGI.Factory1;
using DxgiAdapter = SharpDX.DXGI.Adapter1;
using DxgiDevice = SharpDX.DXGI.Device1;
#endif
#if DESKTOP
using DxgiAdapter = SharpDX.DXGI.Adapter1;
using DxgiDevice = SharpDX.DXGI.Device1;
using DxgiFactory = SharpDX.DXGI.Factory1;
#endif

namespace SeeingSharp.Multimedia.Core
{
    public class EngineAdapterInfo 
    {
        private const string TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO = "Common hardware information";

        private List<EngineOutputInfo> m_outputs;
        private DxgiAdapter m_adapter;
        private int m_adapterIndex;
        private bool m_isSoftware;
        private D3D.FeatureLevel m_d3d11FeatureLevel;
        private DXGI.AdapterDescription m_adapterDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterItemViewModel" /> class.
        /// </summary>
        /// <param name="adapter">The adapter to get all data from.</param>
        internal EngineAdapterInfo(int adapterIndex, DxgiAdapter adapter)
        {
            m_outputs = new List<EngineOutputInfo>();
            m_adapter = adapter;
            m_adapterIndex = adapterIndex;

            m_adapterDescription = adapter.Description;
            m_isSoftware = m_adapterDescription.Description.StartsWith("Microsoft Basic Render Driver");

            m_d3d11FeatureLevel = D3D11.Device.GetSupportedFeatureLevel(adapter);

            //Query for output information
            DXGI.Output[] outputs = adapter.Outputs;
            for (int loop = 0; loop < outputs.Length; loop++)
            {
                try
                {
                    DXGI.Output actOutput = outputs[loop];
                    try
                    {
                        m_outputs.Add(new EngineOutputInfo(loop, actOutput));
                    }
                    finally
                    {
                        actOutput.Dispose();
                    }
                }
                catch (Exception)
                {
                    //Query for output information not possible
                    // .. no special handling needed here
                }
            }
        }

        /// <summary>
        /// Gets all outputs supported by this adapter.
        /// </summary>
        [Browsable(false)]
        public List<EngineOutputInfo> Outputs
        {
            get { return m_outputs; }
        }

        /// <summary>
        /// Gets the corresponding adapter.
        /// </summary>
        [Browsable(false)]
        internal DxgiAdapter Adapter
        {
            get { return m_adapter; }
        }

        /// <summary>
        /// Gets the index of the adapter.
        /// </summary>
        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public int AdapterIndex
        {
            get { return m_adapterIndex; }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public string MaxFeatureLevelD3D11
        {
            get { return m_d3d11FeatureLevel.ToString(); }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public bool IsSoftwareAdapter
        {
            get { return m_isSoftware; }
        }

        /// <summary>
        /// Gets the description of the adapter.
        /// </summary>
        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public string AdapterDescription
        {
            get { return m_adapterDescription.Description; }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public string DedicatedSystemMemory
        {
            get { return m_adapterDescription.DedicatedSystemMemory.ToString(); }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public string DedicatedVideoMemory
        {
            get { return m_adapterDescription.DedicatedVideoMemory.ToString(); }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_HARDWARE_INFO)]
        public string SharedSystemMemory
        {
            get { return m_adapterDescription.SharedSystemMemory.ToString(); }
        }
    }
}