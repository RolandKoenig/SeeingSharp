using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Multimedia.Views;
using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FrozenSky.Util;

namespace FrozenSky.Tutorials.Introduction03
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FrozenSkyPanelPainter m_panelPainter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += OnMainPage_Loaded;
        }

        private async void OnMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_panelPainter != null) { return; }

            // Attach the painter to the target render panel
            m_panelPainter = new FrozenSkyPanelPainter(this.RenderTargetPanel);
            m_panelPainter.RenderLoop.ClearColor = Color4.CornflowerBlue;

            // Build scene graph
            await m_panelPainter.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Define a BACKGROUND layer and configure layer IDs 
                //  => Ensures correct render order
                SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
                manipulator.SetLayerOrderID(bgLayer, 0);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

                // Add the background texture painter to the BACKGROUND layer
                var resBackgroundTexture = manipulator.AddTexture(
                    new AssemblyResourceUriBuilder(
                        "FrozenSky.Tutorials.Introduction03", 
                        true, 
                        "Assets/Textures/Background.png"));
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);

                // Create pallet geometry resource
                PalletType pType = new PalletType();
                pType.ContentColor = Color4.GreenColor;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                // Create pallet object and add it to the scene
                //  => The DEFAULT layer is used by default
                GenericObject palletObject = manipulator.AddGeneric(resPalletGeometry);
                palletObject.Color = Color4.GreenColor;
                palletObject.EnableShaderGeneratedBorder();
                palletObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, EngineMath.RAD_45DEG), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, EngineMath.RAD_45DEG), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, 0f, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => palletObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
            });

            // Configure camera
            Camera3DBase camera = m_panelPainter.Camera;
            camera.Position = new Vector3(2f, 2f, 2f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();
        }
    }
}
