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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;

//Some type mappings
using DxgiAdapter = SharpDX.DXGI.Adapter1;
using DxgiDevice = SharpDX.DXGI.Device1;
using DxgiFactory = SharpDX.DXGI.Factory1;

namespace FrozenSky.Multimedia.Core
{
    public class EngineHardwareInfo 
    {
        private DxgiFactory m_dxgiFactory;
        private List<EngineAdapterInfo> m_adapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineHardwareInfo" /> class.
        /// </summary>
        internal EngineHardwareInfo()
            : this(new DxgiFactory())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableHardwareViewModel" /> class.
        /// </summary>
        internal EngineHardwareInfo(DxgiFactory dxgiFactory)
        {
            m_dxgiFactory = dxgiFactory;
            m_adapters = new List<EngineAdapterInfo>();

            LoadAdapterInformation();
        }

        /// <summary>
        /// Loads all adapter information and builds up all needed view models in a background thread.
        /// </summary>
        private void LoadAdapterInformation()
        {
            int adapterCount = m_dxgiFactory.GetAdapterCount1();
            for (int loop = 0; loop < adapterCount; loop++)
            {
                try
                {
                    DxgiAdapter actAdapter = m_dxgiFactory.GetAdapter1(loop);
                    m_adapters.Add(new EngineAdapterInfo(loop, actAdapter));
                }
                catch (Exception)
                {
                    //No exception handling needed here
                    // .. adapter information simply can not be gathered
                }
            }
        }

        /// <summary>
        /// Gets a collection containing all adapters.
        /// </summary>
        public List<EngineAdapterInfo> Adapters
        {
            get { return m_adapters; }
        }
    }
}
