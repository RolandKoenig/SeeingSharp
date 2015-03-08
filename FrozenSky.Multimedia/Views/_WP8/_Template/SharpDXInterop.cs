//using SharpDX.Direct3D11;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Phone.Input.Interop;
//using Windows.UI.Core;

//namespace FrozenSky.Multimedia.Views
//{
//    internal class SharpDXInterop : IDrawingSurfaceManipulationHandler
//    {
//        public event RequestAdditionalFrameHandler RequestAdditionalFrame;
//        public event RecreateSynchronizedTextureHandler RecreateSynchronizedTexture;

//        public Windows.Foundation.Size NativeResolution { get; set; }
//        public Windows.Foundation.Size _renderResolution;
//        public Windows.Foundation.Size WindowBounds { get; set; }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SharpDXInterop"/> class.
//        /// </summary>
//        public SharpDXInterop()
//        {

//        }

//        public object CreateContentProvider()
//        {
//            var provider = new SharpDXContentProvider(this);
//            return provider;
//        }

//        // IDrawingSurfaceManipulationHandler
//        public void SetManipulationHost(DrawingSurfaceManipulationHost manipulationHost)
//        {
//            manipulationHost.PointerPressed += OnPointerPressed;
//            manipulationHost.PointerMoved += OnPointerMoved;
//            manipulationHost.PointerReleased += OnPointerReleased;
//        }

//        // Event Handlers
//        protected void OnPointerPressed(DrawingSurfaceManipulationHost sender, PointerEventArgs args)
//        {
//            // Insert your code here.
//        }

//        protected void OnPointerMoved(DrawingSurfaceManipulationHost sender, PointerEventArgs args)
//        {
//            // Insert your code here.
//        }

//        protected void OnPointerReleased(DrawingSurfaceManipulationHost sender, PointerEventArgs args)
//        {
//            // Insert your code here.
//        }

//        internal void Connect(DrawingSurfaceRuntimeHost host)
//        {
//            //_renderer = new CubeRenderer();
//            //_renderer.Initialize();
//            //_renderer.UpdateForWindowSizeChange((float)WindowBounds.Width, (float)WindowBounds.Height);
//            //_renderer.UpdateForRenderResolutionChange((float)_renderResolution.Width, (float)_renderResolution.Height);

//            //// Restart timer after renderer has finished initializing.
//            //_timer.Reset();
//        }

//        internal void Disconnect()
//        {
//            //_renderer = null;
//        }

//        internal void PrepareResources(DateTime presentTargetTime, out SharpDX.Bool isContentDirty)
//        {
//            isContentDirty = true;
//        }

//        internal void UpdateForWindowSizeChange(float width, float height)
//        {
//            //_renderer.UpdateForWindowSizeChange(width, height);
//        }

//        internal void GetTexture(SharpDX.Size2F surfaceSize, DrawingSurfaceSynchronizedTexture synchronizedTexture, SharpDX.RectangleF textureSubRectangle)
//        {
//            //_timer.Update();
//            //_renderer.Update(_timer.Total, _timer.Delta);
//            //_renderer.Render();

//            if (RequestAdditionalFrame != null) RequestAdditionalFrame();

//        }

//        internal Texture2D GetTexture()
//        {
//            return null;
//            //return _renderer.GetTexture();
//        }

//        public Windows.Foundation.Size RenderResolution
//        {
//            get { return _renderResolution; }
//            set
//            {
//                if (value.Width != _renderResolution.Width ||
//                    value.Height != _renderResolution.Height)
//                {
//                    _renderResolution = value;

//                    //if (_renderer != null)
//                    //{
//                    //    _renderer.UpdateForRenderResolutionChange((float)_renderResolution.Width, (float)_renderResolution.Height);
//                    //    if (RecreateSynchronizedTexture != null) RecreateSynchronizedTexture();
//                    //}
//                }
//            }
//        }
//    }
//}
