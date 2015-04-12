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
using SharpDX;
//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Drawing3D
{
    public class TypeSafeConstantBufferResource<T> : ConstantBufferResource
        where T : struct
    {
        private T m_initialData;
        private int m_structureSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeConstantBufferResource{T}" /> class.
        /// </summary>
        public TypeSafeConstantBufferResource()
            : base(Utilities.SizeOf<T>())
        {
            m_initialData = new T();
            m_structureSize = Utilities.SizeOf<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeConstantBufferResource{T}" /> class.
        /// </summary>
        public TypeSafeConstantBufferResource(T initialData)
            : base(Utilities.SizeOf<T>())
        {
            m_initialData = initialData;
            m_structureSize = Utilities.SizeOf<T>();
        }

        /// <summary>
        /// Sets given content to the constant buffer.
        /// </summary>
        /// <param name="deviceContext">The context used for updating the constant buffer.</param>
        /// <param name="dataToSet">The data to set.</param>
        internal void SetData(D3D11.DeviceContext deviceContext, T dataToSet)
        {
            DataStream dataStream;
            deviceContext.MapSubresource(base.ConstantBuffer, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None, out dataStream);
            dataStream.Write(dataToSet);
            deviceContext.UnmapSubresource(base.ConstantBuffer, 0);
        }

        /// <summary>
        /// Creates the constant buffer object.
        /// </summary>
        protected internal override D3D11.Buffer CreateConstantBuffer(EngineDevice device)
        {
            using (var dataStream = new SharpDX.DataStream(SharpDX.Utilities.SizeOf<T>(), true, true))
            {
                dataStream.Write(m_initialData);
                dataStream.Position = 0;

                return new D3D11.Buffer(
                    device.DeviceD3D11,
                    dataStream,
                    new D3D11.BufferDescription(
                        m_structureSize,
                        D3D11.ResourceUsage.Dynamic,
                        D3D11.BindFlags.ConstantBuffer,
                        D3D11.CpuAccessFlags.Write,
                        D3D11.ResourceOptionFlags.None,
                        0));
            }
        }
    }
}
