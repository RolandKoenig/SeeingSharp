#region License information (SeeingSharp and all based games/applications)
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

#if UNIVERSAL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;

// Namespace mappings
using D2D = SharpDX.Direct2D1;

namespace SeeingSharp.Multimedia.Drawing2D
{
    public class EffectResource : Drawing2DResourceBase, IEffectInput, IEffectInputInternal
    {
        #region Resources
        private D2D.Effect[] m_loadedEffects;
        #endregion

        #region Configuration
        private IEffectInputInternal[] m_effectInputs;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectResource"/> class.
        /// </summary>
        public EffectResource(params IEffectInput[] effectInputs)
        {
            m_loadedEffects = new D2D.Effect[GraphicsCore.Current.DeviceCount];

            // Get all effect inputs
            m_effectInputs = new IEffectInputInternal[effectInputs.Length];
            for(int loop=0; loop<effectInputs.Length; loop++)
            {
                m_effectInputs[loop] = effectInputs[loop] as IEffectInputInternal;
                if(m_effectInputs[loop] == null)
                {
                    throw new SeeingSharpGraphicsException("Unable to process effectinput at index " + loop + "!");
                }
            }
        }

        /// <summary>
        /// Gets the input object for an effect.
        /// </summary>
        /// <param name="device">The device for which to get the input.</param>
        IDisposable IEffectInputInternal.GetInputObject(EngineDevice device)
        {
            D2D.Effect effect = m_loadedEffects[device.DeviceIndex];
            if(effect == null)
            {
                // Load the effect here
                // TODO: Make this one abstract and implement in base derived classes
                D2D.Effects.GaussianBlur blurEffect = new D2D.Effects.GaussianBlur(device.DeviceContextD2D);
                blurEffect.BorderMode = D2D.BorderMode.Soft;
                blurEffect.Optimization = D2D.GaussianBlurOptimization.Quality;
                blurEffect.StandardDeviation = 5f;
                effect = blurEffect;

                // Set input values
                for(int loop=0; loop<m_effectInputs.Length; loop++)
                {
                    using (D2D.Image actInput = m_effectInputs[loop].GetInputObject(device) as D2D.Image)
                    {
                        effect.SetInput(loop, actInput, new SharpDX.Bool(false));
                    }
                }

                // Store loaded effect
                m_loadedEffects[device.DeviceIndex] = effect;
            }

            return effect.Output;
        }

        /// <summary>
        /// Unloads all resources loaded on the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to unload the resource.</param>
        internal override void UnloadResources(EngineDevice engineDevice)
        {
            D2D.Effect actEffect = m_loadedEffects[engineDevice.DeviceIndex];
            if (actEffect != null)
            {
                GraphicsHelper.DisposeObject(actEffect);
                m_loadedEffects[engineDevice.DeviceIndex] = null;
            }
        }
    }
}

#endif