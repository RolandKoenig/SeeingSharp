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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D2D = SharpDX.Direct2D1;

namespace SeeingSharp.Multimedia.Drawing2D
{
    public class LinearGradientBrushResource : BrushResource
    {
        #region Native resources and configuration
        private LoadedBrushResources[] m_loadedBrushes;
        private Vector2 m_startPoint;
        private Vector2 m_endPoint;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrushResource" /> class.
        /// </summary>
        /// <param name="startPoint">The start point of the gradient.</param>
        /// <param name="endPoint">The end point of the gradient.</param>
        public LinearGradientBrushResource(
            Vector2 startPoint, Vector2 endPoint,
            GradientStop[] gradientStops)
        {
            startPoint.EnsureNotEqual(endPoint, "startPoint", "endPoint");
            gradientStops.EnsureNotNullOrEmpty("gradientStops");

            m_startPoint = startPoint;
            m_endPoint = endPoint;

            m_loadedBrushes = new LoadedBrushResources[GraphicsCore.Current.DeviceCount];

        }

        /// <summary>
        /// Unloads all resources loaded on the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to unload the resource.</param>
        internal override void UnloadResources(EngineDevice engineDevice)
        {
            LoadedBrushResources loadedBrush = m_loadedBrushes[engineDevice.DeviceIndex];
            if (loadedBrush.Brush != null)
            {
                loadedBrush.Brush = GraphicsHelper.DisposeObject(loadedBrush.Brush);
                loadedBrush.GradientStops = GraphicsHelper.DisposeObject(loadedBrush.GradientStops);

                m_loadedBrushes[engineDevice.DeviceIndex] = loadedBrush;
            }
        }

        /// <summary>
        /// Gets the brush for the given device.
        /// </summary>
        /// <param name="engineDevice">The device for which to get the brush.</param>
        internal override D2D.Brush GetBrush(EngineDevice engineDevice)
        {
            // Check for disposed state
            if (base.IsDisposed) { throw new ObjectDisposedException(this.GetType().Name); }

            LoadedBrushResources result = m_loadedBrushes[engineDevice.DeviceIndex];
            if (result.Brush == null)
            {
                // Load the brush

                //result = new D2D.LinearGradientBrush(engineDevice.FakeRenderTarget2D, m_singleColor.ToDXColor());

                //D2D.GradientStopCollection d;

                m_loadedBrushes[engineDevice.DeviceIndex] = result;
            }

            return result.Brush;
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private struct LoadedBrushResources
        {
            public D2D.GradientStopCollection GradientStops;
            public D2D.LinearGradientBrush Brush;
        }
    }
}
