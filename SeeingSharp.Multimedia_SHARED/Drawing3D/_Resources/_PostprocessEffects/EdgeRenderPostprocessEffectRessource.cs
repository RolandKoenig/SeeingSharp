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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class EdgeDetectPostprocessEffectResource : PostprocessEffectResource
    {
        #region Resource keys
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER_BLUR = GraphicsCore.GetNextGenericResourceKey();
        private NamedOrGenericKey KEY_RENDER_TARGET = GraphicsCore.GetNextGenericResourceKey();
        private NamedOrGenericKey KEY_CONSTANT_BUFFER = GraphicsCore.GetNextGenericResourceKey();
        #endregion

        #region Resources
        private RenderTargetTextureResource m_renderTarget;
        private DefaultResources m_defaultResources;
        private PixelShaderResource m_pixelShaderBlur;
        private CBPerObject m_constantBufferData;
        private TypeSafeConstantBufferResource<CBPerObject> m_constantBuffer;
        #endregion

        #region Configuration
        private float m_thickness;
        private bool m_drawOriginalObject;
        private Color4 m_borderColor;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusPostprocessEffectResource"/> class.
        /// </summary>
        public EdgeDetectPostprocessEffectResource()
        {
            m_thickness = 2f;
            m_borderColor = Color4.BlueColor;
            m_drawOriginalObject = true;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            base.LoadResourceInternal(device, resources);

            // Load graphics resources
            m_pixelShaderBlur = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER_BLUR,
                () => GraphicsHelper.GetPixelShaderResource(device, "Postprocessing", "PostprocessEdgeDetect"));
            m_renderTarget = resources.GetResourceAndEnsureLoaded<RenderTargetTextureResource>(
                KEY_RENDER_TARGET,
                () => new RenderTargetTextureResource(RenderTargetCreationMode.Color));
            m_defaultResources = resources.DefaultResources;

            // Load constant buffer
            m_constantBufferData = new CBPerObject();
            m_constantBuffer = resources.GetResourceAndEnsureLoaded<TypeSafeConstantBufferResource<CBPerObject>>(
                KEY_CONSTANT_BUFFER,
                () => new TypeSafeConstantBufferResource<CBPerObject>(m_constantBufferData));
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="resources">Parent ResourceDictionary.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            base.UnloadResourceInternal(device, resources);

            m_pixelShaderBlur = null;
            m_renderTarget = null;
            m_defaultResources = null;
            m_constantBuffer = null;
        }

        /// <summary>
        /// Notifies that rendering begins.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        /// <param name="passID">The ID of the current pass (starting with 0)</param>
        internal override void NotifyBeforeRender(RenderState renderState, int passID)
        {
            switch (passID)
            {
                //******************************
                // 1. Pass: Draw all pixels that ly behind other already rendered elements
                case 0:
                    // Apply current render target size an push render target texture on current rendering stack
                    m_renderTarget.ApplySize(renderState);
                    m_renderTarget.PushOnRenderState(renderState, PushRenderTargetMode.Default_OwnColor_PrevDepthObjectIDNormalDepth);

                    // Clear current render target
                    renderState.ClearCurrentColorBuffer(new Color(0f, 0f, 0f, 0f));
                    break;
            }
        }

        /// <summary>
        /// Notifies that rendering of the plain part has finished.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        /// <param name="passID">The ID of the current pass (starting with 0)</param>
        internal override void NotifyAfterRenderPlain(RenderState renderState, int passID)
        {
            // Nothing to be done here
        }

        /// <summary>
        /// Notifies that rendering has finished.
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        /// <param name="passID">The ID of the current pass (starting with 0)</param>
        /// <returns>
        /// True, if rendering should continue with next pass. False if postprocess effect is finished.
        /// </returns>
        internal override bool NotifyAfterRender(RenderState renderState, int passID)
        {
            D3D11.DeviceContext deviceContext = renderState.Device.DeviceImmediateContextD3D11;

            // Reset settings made on render state (needed for all passes)
            deviceContext.Rasterizer.State = m_defaultResources.RasterStateDefault;

            // Reset render target (needed for all passes)
            m_renderTarget.PopFromRenderState(renderState);

            // Update constant buffer data
            m_constantBufferData.ScreenPixelSize = renderState.ViewInformation.CurrentViewSize.ToVector2();
            m_constantBufferData.Opacity = 0.9f;
            m_constantBufferData.Threshold = 0.2f;
            m_constantBufferData.Thickness = m_thickness;
            m_constantBufferData.BorderColor = m_borderColor.ToVector3();
            m_constantBufferData.OriginalColorAlpha = m_drawOriginalObject ? 1f : 0f;
            m_constantBuffer.SetData(deviceContext, m_constantBufferData);

            // Render result of current pass to the main render target
            switch (passID)
            {
                case 0:
                    base.ApplyAlphaBasedSpriteRendering(deviceContext);
                    try
                    {
                        deviceContext.PixelShader.SetShaderResource(0, m_renderTarget.TextureView);
                        deviceContext.PixelShader.SetSampler(0, m_defaultResources.GetSamplerState(TextureSamplerQualityLevel.Low));
                        deviceContext.PixelShader.SetConstantBuffer(2, m_constantBuffer.ConstantBuffer);
                        deviceContext.PixelShader.Set(m_pixelShaderBlur.PixelShader);
                        deviceContext.Draw(3, 0);
                    }
                    finally
                    {
                        base.DiscardAlphaBasedSpriteRendering(deviceContext);
                    }
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get
            {
                return (m_renderTarget != null) &&
                       (m_renderTarget.IsLoaded);
            }
        }

        public float Thickness
        {
            get { return m_thickness; }
            set { m_thickness = value; }
        }

        public Color4 BorderColor
        {
            get { return m_borderColor; }
            set { m_borderColor = value; }
        }

        public bool DrawOriginalObject
        {
            get { return m_drawOriginalObject; }
            set { m_drawOriginalObject = value; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        [StructLayout(LayoutKind.Sequential)]
        private struct CBPerObject
        {
            public float Opacity;
            public Vector2 ScreenPixelSize;
            public float Thickness;
            public float Threshold;
            public Vector3 BorderColor;
            public float OriginalColorAlpha;
            public Vector3 Dummy;
        }
    }
}
