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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Drawing2D;

namespace SeeingSharp.Multimedia.Components
{
    public class GradientBackgroundComponent
        : ObjectCreatorComponent<GradientBackgroundComponent.PerSceneContext>
    {
        #region Defaults
        private const GradientDirection DEFAULT_GRADIENT_DIR = GradientDirection.TopToBottom;
        private const int DEFAULT_WDITH = 128;
        private const int DEFAULT_HEIGHT = 128;
        private const string DEFAULT_LAYER = "Background";
        private const int DEFAULT_LAYER_ORDER_ID = -10;
        #endregion

        #region Configuration
        private GradientDirection m_gradientDirection;
        private Color4 m_colorStart;
        private Color4 m_colorEnd;
        private int m_textureWidth;
        private int m_textureHeight;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientBackgroundComponent"/> class.
        /// </summary>
        public GradientBackgroundComponent()
        {
            m_gradientDirection = DEFAULT_GRADIENT_DIR;
            m_textureWidth = DEFAULT_WDITH;
            m_textureHeight = DEFAULT_HEIGHT;

            m_colorStart = Color4.WhiteSmoke;
            m_colorEnd = Color4.Gray;

            base.TargetLayer = DEFAULT_LAYER;
            base.TargetLayerOrderID = DEFAULT_LAYER_ORDER_ID;
        }

        /// <summary>
        /// Attaches this component to a scene.
        /// Be careful, this method gets called from a background thread of seeing#!
        /// It may also be called from multiple scenes in parallel or simply withoud previous Detach call.
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene we attach to.</param>
        /// <param name="correspondingView">The view which attached this component.</param>
        protected override PerSceneContext Attach(SceneManipulator manipulator, ViewInformation correspondingView)
        {
            PerSceneContext context = new PerSceneContext();
            switch(m_gradientDirection)
            {
                case GradientDirection.LeftToRight:
                    context.BrushResource = new LinearGradientBrushResource(
                        new System.Numerics.Vector2(0f, 0f),
                        new System.Numerics.Vector2(m_textureWidth, 0f),
                        new GradientStop[]
                        {
                            new GradientStop(m_colorStart, 0f),
                            new GradientStop(m_colorEnd, 1f)
                        });
                    break;

                case GradientDirection.TopToBottom:
                    context.BrushResource = new LinearGradientBrushResource(
                        new System.Numerics.Vector2(0f, 0f),
                        new System.Numerics.Vector2(0f, m_textureHeight),
                        new GradientStop[]
                        {
                            new GradientStop(m_colorStart, 0f),
                            new GradientStop(m_colorEnd, 1f)
                        });
                    break;

                case GradientDirection.Directional:
                    context.BrushResource = new LinearGradientBrushResource(
                        new System.Numerics.Vector2(0f, 0f),
                        new System.Numerics.Vector2(m_textureWidth, m_textureHeight),
                        new GradientStop[]
                        {
                            new GradientStop(m_colorStart, 0f),
                            new GradientStop(m_colorEnd, 1f)
                        });
                    break;
            }

            // Create the background layer if not available already
            base.CreateLayerIfNotAvailable(manipulator);

            // Create and add the background
            context.BackgroundTextureKey = manipulator.AddResource(
                () => new Direct2DSingleRenderTextureResource(context.BrushResource, m_textureWidth, m_textureHeight));
            context.BackgroundPainter = new FullscreenTextureObject(context.BackgroundTextureKey);
            context.BackgroundPainter.Scaling = 1.1f;
            manipulator.Add(context.BackgroundPainter, DEFAULT_LAYER);

            return context;
        }

        /// <summary>
        /// Detaches the specified manipulator.
        /// </summary>
        /// <param name="manipulator">The manipulator.</param>
        /// <param name="correspondingView">The corresponding view.</param>
        /// <param name="context">The context.</param>
        protected override void Detach(SceneManipulator manipulator, ViewInformation correspondingView, PerSceneContext context)
        {
            manipulator.Remove(context.BackgroundPainter);
            manipulator.RemoveResource(context.BackgroundTextureKey);
            GraphicsHelper.SafeDispose(ref context.BrushResource);
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
#endif
        [DefaultValue(DEFAULT_GRADIENT_DIR)]
        public GradientDirection GradientDirection
        {
            get { return m_gradientDirection; }
            set { m_gradientDirection = value; }
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
#endif
        [DefaultValue(DEFAULT_WDITH)]
        public int TextureWidth
        {
            get { return m_textureWidth; }
            set
            {
                m_textureWidth.EnsureValidTextureSize(HardwareDriverLevel.Direct3D9_1, nameof(TextureWidth));
                m_textureWidth = value;
            }
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
#endif
        [DefaultValue(DEFAULT_HEIGHT)]
        public int TextureHeight
        {
            get { return m_textureHeight; }
            set
            {
                m_textureHeight.EnsureValidTextureSize(HardwareDriverLevel.Direct3D9_1, nameof(TextureHeight));
                m_textureHeight = value;
            }
        }

#if UNIVERSAL
        public Windows.UI.Color ColorStart
        {
            get { return m_colorStart.ToWindowsColor(); }
            set { m_colorStart = new Color4(value); }
        }

        public Windows.UI.Color ColorEnd
        {
            get { return m_colorEnd.ToWindowsColor(); }
            set { m_colorEnd = new Color4(value); }
        }
#endif

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
        public System.Drawing.Color DrawingColorStart
        {
            get { return m_colorStart.ToGdiColor(); }
            set { m_colorStart = new Color4(value); }
        }

        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
        public System.Drawing.Color DrawingColorEnd
        {
            get { return m_colorEnd.ToGdiColor(); }
            set { m_colorEnd = new Color4(value); }
        }

        [Browsable(false)]
        public System.Windows.Media.Color WpfColorStart
        {
            get { return m_colorStart.ToWpfColor(); }
            set { m_colorStart = new Color4(value); }
        }

        [Browsable(false)]
        public System.Windows.Media.Color WpfColorEnd
        {
            get { return m_colorEnd.ToWpfColor(); }
            set { m_colorEnd = new Color4(value); }
        }
#endif
        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        public class PerSceneContext
        {
            public FullscreenTextureObject BackgroundPainter;
            public NamedOrGenericKey BackgroundTextureKey;
            public LinearGradientBrushResource BrushResource;
        }
    }
}
