#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public abstract class GameObject2D : SceneObject
    {
        /// <summary>
        /// Updates this object for the given view.
        /// </summary>
        /// <param name="updateState">Current state of the update pass.</param>
        /// <param name="layerViewSubset">The layer view subset wich called this update method.</param>
        protected override void UpdateForViewInternal(SceneRelatedUpdateState updateState, ViewRelatedSceneLayerSubset layerViewSubset)
        {
            if (base.CountRenderPassSubscriptions(layerViewSubset) == 0)
            {
                base.SubscribeToPass(
                    RenderPassInfo.PASS_2D_OVERLAY,
                    layerViewSubset,
                    OnRender_2DOverlay,
                    zOrder: this.RenderZOrder);
            }
        }

        /// <summary>
        /// Loads all resources of the object.
        /// </summary>
        /// <param name="device">Current graphics device.</param>
        /// <param name="resourceDictionary">Current resource dicionary.</param>
        public override sealed void LoadResources(EngineDevice device, ResourceDictionary resourceDictionary)
        {
            // Not needed for 2D objects
        }

        /// <summary>
        /// Are resources loaded for the given device?
        /// </summary>
        public override sealed bool IsLoaded(EngineDevice device)
        {
            // Not needed for 2D objects
            return true;
        }
        
        /// <summary>
        /// Contains all 2D rendering logic for this object.
        /// </summary>
        protected abstract void OnRender_2DOverlay(RenderState renderState);

        /// <summary>
        /// The z order for rendering (2D rendering is sorted by this value).
        /// </summary>
        protected virtual int RenderZOrder
        {
            get { return 0; }
        }
    }
}
