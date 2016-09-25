#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SeeingSharp;

namespace SeeingSharpModelViewer
{
    public class MiscGraphicsObjectsViewModel : ViewModelBase
    {
        private Scene m_scene;
        private SceneLayer m_bgLayer;

        public MiscGraphicsObjectsViewModel(Scene scene)
        {
            scene.EnsureNotNull(nameof(scene));

            m_scene = scene;
        }

        /// <summary>
        /// Initializes the scene for this model viewer.
        /// </summary>
        internal async Task InitializeAsync()
        {
            await m_scene.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgImageLayer = manipulator.AddLayer("BACKGROUND_FLAT");
                SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
                manipulator.SetLayerOrderID(bgImageLayer, 0);
                manipulator.SetLayerOrderID(bgLayer, 1);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 2);

                // Store a reference to the background layer
                m_bgLayer = bgLayer;

                // Define background texture
                ResourceLink linkBackgroundTexture = new AssemblyResourceUriBuilder(
                    "SeeingSharpModelViewer", true,
                    "Assets/Textures/Background.dds");
                NamedOrGenericKey resBackgroundTexture = manipulator.AddTexture(linkBackgroundTexture);
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgImageLayer.Name);

                // Define ground
                Grid3DType objTypeGrid = new Grid3DType();
                objTypeGrid.TilesX = 64;
                objTypeGrid.TilesZ = 64;
                objTypeGrid.HighlightXZLines = true;
                objTypeGrid.TileWidth = 0.25f;
                objTypeGrid.GroupTileCount = 4;
                objTypeGrid.GenerateGround = false;
                objTypeGrid.XLineHighlightColor = Color4.GreenColor;
                objTypeGrid.ZLineHighlightColor = Color4.BlueColor;

                NamedOrGenericKey resGridGeometry = manipulator.AddGeometry(objTypeGrid);
                manipulator.Add(new GenericObject(resGridGeometry), "BACKGROUND");

                TextGeometryOptions textXOptions = TextGeometryOptions.Default;
                textXOptions.SurfaceVertexColor = Color4.GreenColor;
                textXOptions.MakeVolumetricText = false;
                textXOptions.FontSize = 30;
                GenericObject textX = manipulator.Add3DText(
                    "X", textXOptions,
                    realignToCenter: true, 
                    layer: "BACKGROUND");
                textX.Position = new Vector3(9f, 0, 0);

                TextGeometryOptions textZOptions = TextGeometryOptions.Default;
                textZOptions.SurfaceVertexColor = Color4.BlueColor;
                textZOptions.MakeVolumetricText = false;
                textZOptions.FontSize = 30;
          
                GenericObject textZ = manipulator.Add3DText(
                    "Z", textZOptions, 
                    realignToCenter: true,
                    layer: "BACKGROUND");
                textZ.Position = new Vector3(0f, 0f, 9f);
            });
        }

        public bool BackgroundVisible
        {
            get
            {
                if (m_bgLayer == null) { return false; }
                else { return m_bgLayer.IsRenderingEnabled; }
            }
            set
            {
                if(m_bgLayer == null) { return; }

                if(m_bgLayer.IsRenderingEnabled != value)
                {
                    m_bgLayer.IsRenderingEnabled = value;
                    RaisePropertyChanged(nameof(BackgroundVisible));
                }
            }
        }
    }
}
