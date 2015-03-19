using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Util;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Kinect
{
    public class KinectSceletonStreamPresenter : IDisposable
    {
        // Keys for graphics resources
        private static readonly NamedOrGenericKey RES_KEY_CIRCLE = GraphicsCore.GetNextGenericResourceKey();

        // Data that has to be disposed 
        #region
        private Scene m_bodyScene;
        private List<MessageSubscription> m_messageSubscriptions;
        #endregion

        // Cached sceleton data
        #region
        private List<Body> m_bodyData;
        private volatile bool m_bodyDataModified;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectSceletonStreamPresenter"/> class.
        /// </summary>
        public KinectSceletonStreamPresenter()
        {
            m_bodyData = new List<Body>();
            m_bodyDataModified = false;

            // Prepare scene object
            m_bodyScene = new Scene();
            m_bodyScene.ManipulateSceneAsync(OnBodyScene_Initialize);

            // Get the MessageHandler of the KinectThread
            FrozenSkyMessageHandler kinectMessageHandler =
                FrozenSkyMessageHandler.GetForThread(Constants.KINECT_THREAD_NAME);

            // Subscribe to messages from kinect
            m_messageSubscriptions = new List<MessageSubscription>();
            m_messageSubscriptions.Add(
                kinectMessageHandler.Subscribe<MessageBodyFrameArrived>(OnMessage_BodyFrameArrived));
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            m_messageSubscriptions.ForEach((actSubscription) => actSubscription.Unsubscribe());
            m_messageSubscriptions.Clear();
        }

        /// <summary>
        /// Updates the given body model.
        /// Be carefull.. this method runs within 3D-Engine update thread.
        /// </summary>
        /// <param name="scene">The scene to be manipulated.</param>
        /// <param name="manipulator">The manipulator.</param>
        /// <param name="bodyObject">The body object.</param>
        /// <param name="bodyIndex">Index of the body.</param>
        private static void UpdateBodyModel(Scene scene, SceneManipulator manipulator, Body bodyObject, int bodyIndex)
        {
            // Ensure we have a layer for this body
            string actBodyLayerName = "Body_" + bodyIndex;
            SceneLayer actBodyLayer = manipulator.TryGetLayer(actBodyLayerName);
            if(actBodyLayer == null)
            {
                actBodyLayer = manipulator.AddLayer(actBodyLayerName);
                manipulator.SetLayerOrderID(actBodyLayer, bodyIndex);

                
            }
        }

        /// <summary>
        /// This method is called after the scene was created.
        /// Be carefull: This method run's in 3D-Engines update thread.
        /// </summary>
        /// <param name="manipulator">The manipulator.</param>
        private void OnBodyScene_Initialize(SceneManipulator manipulator)
        {
            SceneLayer bgLayer = manipulator.AddLayer("BACKGROUND");
            manipulator.SetLayerOrderID(bgLayer, 0);
            manipulator.SetLayerOrderID(Scene.DEFAULT_LAYER_NAME, 1);

            ResourceSource sourceWallTexture = new Uri(
                "/FrozenSky.ModelViewer;component/Resources/Textures/Background.png",
                UriKind.Relative);
            var resBackgroundTexture = manipulator.AddTexture(sourceWallTexture);
            manipulator.Add(new TexturePainter(resBackgroundTexture), bgLayer.Name);

            FloorType floorType = new FloorType(new Vector2(3f, 3f), 0.1f);
            floorType.SetTilemap(40, 40);
            floorType.DefaultFloorMaterial = manipulator.AddSimpleColoredMaterial(new Uri(
                "/FrozenSky.ModelViewer;component/Resources/Textures/Floor.png",
                UriKind.Relative));
            var resGeometry = manipulator.AddResource<GeometryResource>(() => new GeometryResource(floorType));
            GenericObject floorObject = manipulator.AddGeneric(resGeometry);
            floorObject.IsPickingTestVisible = false;
        }

        /// <summary>
        /// Called when a body frame is received.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage_BodyFrameArrived(MessageBodyFrameArrived message)
        {
            if (m_bodyScene.CountViews <= 0) { return; }
            if (m_bodyDataModified) { return; }

            using (BodyFrame bodyFrame = message.BodyFrameArgs.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null) { return; }
  
                // Process incoming body frame
                //  1. Store data in local m_bodyData liste
                bodyFrame.GetAndRefreshBodyData(m_bodyData);
                m_bodyDataModified = true;

                //  2. Modify 3D scene based on the data 
                m_bodyScene.ManipulateSceneAsync((manipulator) =>
                {
                    try
                    {
                        for(int actBodyIndex = 0; actBodyIndex<m_bodyData.Count; actBodyIndex++)
                        {
                            UpdateBodyModel(m_bodyScene, manipulator, m_bodyData[actBodyIndex], actBodyIndex);
                        }
                    }
                    finally
                    {
                        m_bodyDataModified = false;
                    }
                });
            }
        }

        /// <summary>
        /// Gets the scene which is presenting body data.
        /// </summary>
        public Scene BodyScene
        {
            get { return m_bodyScene; }
        }
    }
}
