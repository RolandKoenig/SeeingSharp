using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Multimedia.Views;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Humanizer;


namespace FrozenSky.Tutorials.Introduction04
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Members for painting
        #region
        private FrozenSkyPanelPainter m_panelPainter;
        #endregion

        // Members for picking
        #region
        private DispatcherTimer m_pickingTimer;
        private bool m_isPicking;
        private bool m_isMouseInside;
        private uint m_mouseID;
        private Point m_mousePosition;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            m_pickingTimer = new DispatcherTimer();
            m_pickingTimer.Tick += OnPickingTimer_Tick;
            m_pickingTimer.Interval = TimeSpan.FromMilliseconds(100.0);
            m_pickingTimer.Start();

            this.Loaded += OnMainPage_Loaded;
            this.PointerEntered += OnMainPage_PointerEntered;
            this.PointerMoved += OnMainPage_PointerMoved;
            this.PointerExited += OnMainPage_PonterExited;
        }

        /// <summary>
        /// Stores current mouse position.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void OnMainPage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse) { return; }

            m_isMouseInside = true;
            m_mouseID = e.Pointer.PointerId;
            m_mousePosition = Point.FromWinRTPoint(e.GetCurrentPoint(this).Position);
        }

        /// <summary>
        /// Stores current mouse position.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void OnMainPage_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse) { return; }

            m_isMouseInside = true;
            m_mouseID = e.Pointer.PointerId;
            m_mousePosition = Point.FromWinRTPoint(e.GetCurrentPoint(this).Position);
        }

        /// <summary>
        /// Stores current mouse position.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void OnMainPage_PonterExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse) { return; }

            m_isMouseInside = false;
            m_mouseID = 0;
            m_mousePosition = Point.Empty;
        }

        /// <summary>
        /// Performs picking on timer tick event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void OnPickingTimer_Tick(object sender, object e)
        {
            if (m_isPicking) { return; }
            if (m_panelPainter == null) { return; }
            if (!m_isMouseInside) { return; }

            m_isPicking = true;
            try
            {
                if (m_mousePosition == Point.Empty) { return; }

                // The main picking call
                // (needs the mouse position and the detail level
                List<SceneObject> pickedObjects = await m_panelPainter.RenderLoop.PickObjectAsync(
                    m_mousePosition,
                    new PickingOptions() { OnlyCheckBoundingBoxes = false });

                if (pickedObjects.Count > 0)
                {
                    this.TxtPickedObject.Text = pickedObjects
                        .Select((actObject) => actObject.Tag1)
                        .ToCommaSeparatedString();
                }
                else
                {
                    this.TxtPickedObject.Text = "none";
                }
            }
            finally
            {
                m_isPicking = false;
            }
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
                        "FrozenSky.Tutorials.Introduction04", 
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
                for(int loopX=0 ; loopX<11; loopX++)
                {
                    for(int loopY=0; loopY < 11; loopY++)
                    {
                        GenericObject palletObject = manipulator.AddGeneric(resPalletGeometry);
                        palletObject.Color = Color4.GreenColor;
                        palletObject.Position = new Vector3(
                            -10f + loopX * 2f,
                            -10f + loopY * 2f,
                            0f);
                        palletObject.Tag1 = "Pallet (X={0}, Y={1})".FormatWith(loopX, loopY);
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
                    }
                }
            });

            // Configure camera
            Camera3DBase camera = m_panelPainter.Camera;
            camera.Position = new Vector3(0f, 0f, -25f);
            camera.Target = new Vector3(0f, 0f, 0f);
            camera.UpdateCamera();
        }
    }
}
