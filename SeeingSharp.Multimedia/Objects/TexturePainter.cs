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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Objects
{
    public class TexturePainter : SceneObject
    {
        private NamedOrGenericKey m_resTexture;
        private float m_scaling;

        private IndexBasedDynamicCollection<TexturePainterHelper> m_texturePainterHelpers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturePainter"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public TexturePainter(NamedOrGenericKey texture)
        {
            m_resTexture = texture;

            m_scaling = 1f;

            m_texturePainterHelpers = new IndexBasedDynamicCollection<TexturePainterHelper>();
        }

        /// <summary>
        /// Loads all resources of the object.
        /// </summary>
        /// <param name="device">Current DirectX device.</param>
        public override void LoadResources(EngineDevice device, ResourceDictionary resourceDictionary)
        {
            TexturePainterHelper newHelper = new TexturePainterHelper(m_resTexture);

            m_texturePainterHelpers.AddObject(
                newHelper,
                device.DeviceIndex);

            newHelper.LoadResources(resourceDictionary);
        }

        /// <summary>
        /// Are resources loaded for the given device?
        /// </summary>
        /// <param name="device"></param>
        public override bool IsLoaded(EngineDevice device)
        {
            return m_texturePainterHelpers.HasObjectAt(device.DeviceIndex);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {
            
        }

        /// <summary>
        /// Updates this object for the given view.
        /// </summary>
        /// <param name="updateState">Current state of the update pass.</param>
        /// <param name="layerViewSubset">The layer view subset wich called this update method.</param>
        protected override void UpdateForViewInternal(UpdateState updateState, ViewRelatedSceneLayerSubset layerViewSubset)
        {
            //Subscribe to render passes
            if (base.CountRenderPassSubscriptions(layerViewSubset) == 0)
            {
                base.SubscribeToPass(
                    RenderPassInfo.PASS_PLAIN_RENDER,
                    layerViewSubset, OnRenderPlain);
            }
        }

        /// <summary>
        /// Renders the object.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        private void OnRenderPlain(RenderState renderState)
        {
            TexturePainterHelper actHelper = m_texturePainterHelpers[renderState.DeviceIndex];
            actHelper.Scaling = m_scaling;
            actHelper.RenderPlain(renderState);
        }

        /// <summary>
        /// Unloads all resources of the object.
        /// </summary>
        public override void UnloadResources()
        {
            base.UnloadResources();

            foreach(TexturePainterHelper actHelper in m_texturePainterHelpers)
            {
                actHelper.UnloadResources();
            }

            m_texturePainterHelpers.Clear();
        }

        /// <summary>
        /// Gets or sets the scaling factor.
        /// </summary>
        public float Scaling
        {
            get { return m_scaling; }
            set { m_scaling = value; }
        }
    }
}
