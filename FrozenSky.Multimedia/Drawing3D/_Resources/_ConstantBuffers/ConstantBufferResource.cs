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

using FrozenSky.Multimedia.Core;
using System;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class ConstantBufferResource : Resource
    {
        private D3D11.Buffer m_constantBuffer;
        private int m_bufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantBufferResource" /> class.
        /// </summary>
        public ConstantBufferResource(int bufferSize)
        {
            if (bufferSize < 1) { throw new ArgumentException("Invalid value for buffer size!", "bufferSize"); }
            m_bufferSize = bufferSize;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_constantBuffer = CreateConstantBuffer(device);
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_constantBuffer = GraphicsHelper.DisposeObject(m_constantBuffer);
        }

        /// <summary>
        /// Creates the constant buffer object.
        /// </summary>
        protected internal virtual D3D11.Buffer CreateConstantBuffer(EngineDevice device)
        {
            return new D3D11.Buffer(
                device.DeviceD3D11,
                m_bufferSize,
                D3D11.ResourceUsage.Dynamic,
                D3D11.BindFlags.ConstantBuffer,
                D3D11.CpuAccessFlags.Write,
                D3D11.ResourceOptionFlags.None,
                0);
        }

        /// <summary>
        /// Is the buffer loaded correctly?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_constantBuffer != null; }
        }

        /// <summary>
        /// Gets the buffer object.
        /// </summary>
        internal D3D11.Buffer ConstantBuffer
        {
            get { return m_constantBuffer; }
        }

        /// <summary>
        /// Gets the total size of the constant buffer.
        /// </summary>
        public int BufferSize
        {
            get { return m_bufferSize; }
        }
    }
}
