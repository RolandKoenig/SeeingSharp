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
        #region Resources
        private LoadedBrushResources[] m_loadedBrushes;
        #endregion

        #region Configuration
        private GradientStop[] m_gradientStops;
        private ExtendMode m_extendMode;
        private Gamma m_gamma;
        private Vector2 m_startPoint;
        private Vector2 m_endPoint;
        private float m_opacity;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrushResource" /> class.
        /// </summary>
        /// <param name="startPoint">The start point of the gradient.</param>
        /// <param name="endPoint">The end point of the gradient.</param>
        /// <param name="gradientStops">All points within the color gradient.</param>
        /// <param name="extendMode">How to draw outside the content area?</param>
        /// <param name="gamma">The gama configuration.</param>
        /// <param name="opacity">The opacity value of the brush.</param>
        public LinearGradientBrushResource(
            Vector2 startPoint, Vector2 endPoint,
            GradientStop[] gradientStops,
            ExtendMode extendMode = ExtendMode.Clamp,
            Gamma gamma = Gamma.StandardRgb,
            float opacity = 1f)
        {
            startPoint.EnsureNotEqual(endPoint, nameof(startPoint), nameof(endPoint));
            gradientStops.EnsureNotNullOrEmpty(nameof(gradientStops));

            m_gradientStops = gradientStops;
            m_startPoint = startPoint;
            m_endPoint = endPoint;
            m_extendMode = extendMode;
            m_gamma = gamma;
            m_opacity = opacity;

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
                // Convert gradient stops to structure from SharpDX
                D2D.GradientStop[] d2dGradientStops = new D2D.GradientStop[m_gradientStops.Length];
                for(int loop=0; loop<d2dGradientStops.Length; loop++)
                {
                    d2dGradientStops[loop] = new D2D.GradientStop()
                    {
                        Color = m_gradientStops[loop].Color.ToDXColor(),
                        Position = m_gradientStops[loop].Position
                    };
                }

                // Create the brush
                result = new LoadedBrushResources();
                result.GradientStops = new D2D.GradientStopCollection(
                    engineDevice.FakeRenderTarget2D,
                    d2dGradientStops,
                    (D2D.Gamma)m_gamma,
                    (D2D.ExtendMode)m_extendMode);

                unsafe
                {
                    Matrix3x2 identityMatrix = Matrix3x2.Identity;
                    result.Brush = new D2D.LinearGradientBrush(
                        engineDevice.FakeRenderTarget2D,
                        new D2D.LinearGradientBrushProperties()
                        {
                            StartPoint = m_startPoint.ToDXVector(),
                            EndPoint = m_endPoint.ToDXVector()
                        },
                        new D2D.BrushProperties()
                        {
                            Opacity = m_opacity,
                            Transform = *(SharpDX.Mathematics.Interop.RawMatrix3x2*)&identityMatrix
                        },
                        result.GradientStops);
                }

                m_loadedBrushes[engineDevice.DeviceIndex] = result;
            }

            return result.Brush;
        }

        public Gamma Gamma
        {
            get { return m_gamma; }
        }

        public ExtendMode ExtendMode
        {
            get { return m_extendMode; }
        }

        public Vector2 StartPoint
        {
            get { return m_startPoint; }
        }

        public Vector2 EndPoint
        {
            get { return m_endPoint; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// A simple helper storing both resurces.. 
        ///  - the GradientStopCollection
        ///  - and the LinearGradientBrush itself
        /// </summary>
        private struct LoadedBrushResources
        {
            public D2D.GradientStopCollection GradientStops;
            public D2D.LinearGradientBrush Brush;
        }
    }
}
