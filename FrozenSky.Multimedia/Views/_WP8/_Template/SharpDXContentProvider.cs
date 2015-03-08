//using SharpDX.Direct3D11;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FrozenSky.Multimedia.Views
//{
//    /// <summary>
//    /// This is a port of Direct3D C++ WP8 sample. This port is not clean and complete.
//    /// The preferred way to access Direct3D on WP8 is by using SharpDX.Toolkit.
//    /// </summary>
//    internal class SharpDXContentProvider : DrawingSurfaceContentProviderNativeBase
//    {
//        private SharpDXInterop m_controller;
//        private DrawingSurfaceRuntimeHost m_host;
//        private DrawingSurfaceSynchronizedTexture m_synchronizedTexture;
//        private SharpDX.RectangleF m_textureSubRectangle;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SharpDXContentProvider"/> class.
//        /// </summary>
//        /// <param name="controller">The controller.</param>
//        public SharpDXContentProvider(SharpDXInterop controller)
//        {
//            m_controller = controller;

//            m_controller.RequestAdditionalFrame += () =>
//            {
//                if (m_host != null)
//                {
//                    m_host.RequestAdditionalFrame();
//                }
//            };

//            m_controller.RecreateSynchronizedTexture += () =>
//            {
//                if (m_host != null)
//                {
//                    m_synchronizedTexture = m_host.CreateSynchronizedTexture(m_controller.GetTexture());
//                }
//            };

//        }

//        public override void Connect(DrawingSurfaceRuntimeHost host)
//        {
//            m_host = host;
//            m_controller.Connect(host);
//        }

//        public override void Disconnect()
//        {
//            m_controller.Disconnect();
//            m_host = null;
//            SharpDX.Utilities.Dispose(ref m_synchronizedTexture);
//        }

//        public override void PrepareResources(DateTime presentTargetTime, out SharpDX.Bool isContentDirty)
//        {
//            m_controller.PrepareResources(presentTargetTime, out isContentDirty);

//        }


//        public override void GetTexture(SharpDX.Size2F surfaceSize, out DrawingSurfaceSynchronizedTexture synchronizedTexture, out SharpDX.RectangleF textureSubRectangle)
//        {
//            if (m_synchronizedTexture == null)
//            {
//                m_synchronizedTexture = m_host.CreateSynchronizedTexture(m_controller.GetTexture());
//            }

//            // Set output parameters.
//            m_textureSubRectangle.Left = 0.0f;
//            m_textureSubRectangle.Top = 0.0f;
//            m_textureSubRectangle.Right = surfaceSize.Width;
//            m_textureSubRectangle.Bottom = surfaceSize.Height;


//            //HOW DO YOU DO A Microsoft::WRL::ComPtr<T>   CopyTo ?????
//            //m_synchronizedTexture.CopyTo(synchronizedTexture);
//            synchronizedTexture = m_synchronizedTexture;

//            textureSubRectangle = m_textureSubRectangle;

//            //something is going wrong here as the second time thru the BeginDraw consumes 
//            //the call and controlnever returns back to this method, thus GetTexture 
//            //(the call after begindraw) never fires again... ??????
//            synchronizedTexture.BeginDraw();

//            m_controller.GetTexture(surfaceSize, synchronizedTexture, textureSubRectangle);

//            synchronizedTexture.EndDraw();
//        }
//    }
//}
