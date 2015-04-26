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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class SceneRenderParameters : Resource
    {
        internal NamedOrGenericKey KEY_CONSTANT_BUFFER = GraphicsCore.GetNextGenericResourceKey();

        private TypeSafeConstantBufferResource<CBPerFrame> m_cbPerFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneRenderParameters" /> class.
        /// </summary>
        internal SceneRenderParameters()
        {

        }

        /// <summary>
        /// Updates all parameters.
        /// </summary>
        /// <param name="renderState">The render state on which to apply.</param>
        /// <param name="cbPerFrame">Per frame data.</param>
        internal void UpdateValues(RenderState renderState, CBPerFrame cbPerFrame)
        {
            m_cbPerFrame.SetData(renderState.Device.DeviceImmediateContextD3D11, cbPerFrame);
        }

        /// <summary>
        /// Applies all parameters.
        /// </summary>
        /// <param name="renderState">The render state on which to apply.</param>
        internal IDisposable Apply(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            //TODO: Update contents of the constant buffer

            // Apply constant buffer on shaders
            deviceContext.VertexShader.SetConstantBuffer(0, m_cbPerFrame.ConstantBuffer);
            deviceContext.PixelShader.SetConstantBuffer(0, m_cbPerFrame.ConstantBuffer);

            return new DummyDisposable(() => Discard(renderState));
        }

        /// <summary>
        /// Discards all parameters.
        /// </summary>
        /// <param name="renderState">The render state on which to discard.</param>
        internal void Discard(RenderState renderState)
        {
            //// Discard constant buffer on shaders
            //renderState.DeviceContext.VertexShader.SetConstantBuffer(0, null);
            //renderState.DeviceContext.PixelShader.SetConstantBuffer(0, null);
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_cbPerFrame = resources.GetResourceAndEnsureLoaded(
                KEY_CONSTANT_BUFFER,
                () => new TypeSafeConstantBufferResource<CBPerFrame>());
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_cbPerFrame = null;

            //resources.RemoveResource(KEY_CONSTANT_BUFFER);
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_cbPerFrame != null; }
        }
    }
}
