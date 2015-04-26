using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeeingSharp.RKKinectLounge.Modules.Multimedia
{
    /// <summary>
    /// Interaktionslogik für Object3DView.xaml
    /// </summary>
    public partial class Object3DView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Object3DView"/> class.
        /// </summary>
        public Object3DView()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Called when the control has to load itself.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Return here if the scene is already initialized
            if (CtrlView3D.Scene.CountObjects > 0) { return; }

            // Configure camera
            Camera3DBase camera = CtrlView3D.Camera as Camera3DBase;
            camera.Position = new Vector3(4f, 4f, 4f);
            camera.Target = new Vector3(0f, 0f, 0f);
            camera.UpdateCamera();

            // Build default scene
            await CtrlView3D.Scene.ManipulateSceneAsync((manipulator) =>
            {
                SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
                manipulator.SetLayerOrderID(bgLayer, 0);
                manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

                // Define background object
                var resBackgroundTexture = manipulator.AddTexture(new Uri(
                    "/SeeingSharp.RKKinectLounge;component/Assets/Textures/Background.png",
                    UriKind.Relative));
                manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);

                // Define floor
                FloorType floorType = new FloorType(new Vector2(3f, 3f), 0.1f);
                floorType.SetTilemap(40, 40);
                floorType.DefaultFloorMaterial = manipulator.AddSimpleColoredMaterial(new Uri(
                    "/SeeingSharp.RKKinectLounge;component/Assets/Textures/Floor.png",
                    UriKind.Relative));
                var resGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(floorType));
                GenericObject floorObject = manipulator.AddGeneric(resGeometry);
                floorObject.IsPickingTestVisible = false;

                // Create pallet geometry resource
                PalletType pType = new PalletType();
                pType.ContentColor = Color4.Transparent;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                // Create pallet object
                GenericObject palletObject = manipulator.AddGeneric(resPalletGeometry);
                palletObject.Color = Color4.GreenColor;
                palletObject.EnableShaderGeneratedBorder();
                palletObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => palletObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
            });
        }
    }
}
