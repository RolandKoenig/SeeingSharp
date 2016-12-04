using SharpDX.Direct3D11;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.Input.Interop;
using SeeingSharp.Multimedia.Core;

// Some namespace mappings
using SDX = SharpDX;
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Views
{
    public class WP8DrawingSurfaceInterop : DrawingSurfaceContentProviderNativeBase, IDrawingSurfaceManipulationHandler
    {
        // Parent painter object
        private SeeingSharpDrawingSurfacePainter m_surfacePainter;

        // Windows phone host objects
        private DrawingSurfaceManipulationHost m_manipulationHost;
        private DrawingSurfaceRuntimeHost m_runtimeHost;

        // Direct3D resources
        private DrawingSurfaceSynchronizedTexture m_backBufferWP8Sync;
        private D3D11.Texture2D m_backBufferWP8;
        private Size2 m_lastPixelSize;
        private EngineDevice m_lastDevice;

        // Helper variable for notifying new frames
        private bool m_contentDirty;
        private int m_frameRequestCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="WP8DrawingSurfaceInterop"/> class.
        /// </summary>
        /// <param name="surfacePainter">The surface painter.</param>
        public WP8DrawingSurfaceInterop(SeeingSharpDrawingSurfacePainter surfacePainter)
        {
            m_surfacePainter = surfacePainter;
        }

        /// <summary>
        /// Requests next rendering.
        /// </summary>
        internal void RequestNextFrame()
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            Interlocked.Increment(ref m_frameRequestCount);

            m_contentDirty = true;
            if (m_runtimeHost != null) { m_runtimeHost.RequestAdditionalFrame(); }
        }

        /// <summary>
        /// Creates a ContentProvider object.
        /// </summary>
        public object CreateContentProvider()
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            return this;
        }

        /// <summary>
        /// Wird aufgerufen, wenn der Bearbeitungshandler für ein DrawingSurface- oder DrawingSurfaceBackgroundGrid-Steuerelement festgelegt wird.
        /// </summary>
        /// <param name="manipulationHost">Das Objekt, das die Bearbeitungsereignisse von der Zeichenoberfläche empfängt.</param>
        public void SetManipulationHost(DrawingSurfaceManipulationHost manipulationHost)
        {
            if (m_surfacePainter == null) { return; }

            if(m_manipulationHost != null)
            {
                m_manipulationHost.PointerMoved -= OnManipulationHostPointerMoved;
                m_manipulationHost.PointerPressed -= OnManipulationHostPointerPressed;
                m_manipulationHost.PointerReleased -= OnManipulationHostPointerReleased;
            }
            m_manipulationHost = manipulationHost;
            if (m_manipulationHost != null)
            {
                m_manipulationHost.PointerMoved += OnManipulationHostPointerMoved;
                m_manipulationHost.PointerPressed += OnManipulationHostPointerPressed;
                m_manipulationHost.PointerReleased += OnManipulationHostPointerReleased;
            }
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public override void Connect(DrawingSurfaceRuntimeHost host)
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            m_runtimeHost = host;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public override void Disconnect()
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            // Dispose previously created buffers
            GraphicsHelper.SafeDispose(ref m_backBufferWP8Sync);
            GraphicsHelper.SafeDispose(ref m_backBufferWP8);

            m_runtimeHost = null;
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="surfaceSize">Size of the surface.</param>
        /// <param name="synchronizedTexture">The synchronized texture.</param>
        /// <param name="textureSubRectangle">The texture sub rectangle.</param>
        public override void GetTexture(SDX.Size2F surfaceSize, out DrawingSurfaceSynchronizedTexture synchronizedTexture, out SDX.RectangleF textureSubRectangle)
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            EngineDevice currentDevice = m_surfacePainter.RenderLoop.Device;
            Size2 pixelSize = m_surfacePainter.GetCurrentRenderTargetSize();

            // Create the synchronized texture if needed
            if((m_backBufferWP8Sync == null) ||
               (m_lastPixelSize != pixelSize) ||
               (m_lastDevice != currentDevice))
            {
                // Dispose previously created buffers
                GraphicsHelper.SafeDispose(ref m_backBufferWP8Sync);
                GraphicsHelper.SafeDispose(ref m_backBufferWP8);

                // Create backbuffers for xaml rendering
                m_backBufferWP8 = GraphicsHelper.CreateSharedTextureWP8Xaml(currentDevice, pixelSize.Width, pixelSize.Height);
                m_backBufferWP8Sync = m_runtimeHost.CreateSynchronizedTexture(m_backBufferWP8);

                m_lastPixelSize = pixelSize;
                m_lastDevice = currentDevice;
            }

            // Copy contents of current back buffer to synchronized texture
            m_backBufferWP8Sync.BeginDraw();
            try
            {
                D3D11.Texture2D sourceRenderTarget = m_surfacePainter.RenderTargetForSynchronization;

                currentDevice.DeviceImmediateContextD3D11.ResolveSubresource(
                    sourceRenderTarget, 0, m_backBufferWP8, 0, GraphicsHelper.DEFAULT_TEXTURE_FORMAT_SHARING);
                currentDevice.DeviceImmediateContextD3D11.Flush();
            }
            finally
            {
                m_backBufferWP8Sync.EndDraw();
            }

            // Reset request counter
            m_frameRequestCount = 0;

            // Set return parameters
            synchronizedTexture = m_backBufferWP8Sync;
            textureSubRectangle = new SDX.RectangleF(0f, 0f, surfaceSize.Width, surfaceSize.Height);
        }

        /// <summary>
        /// Prepares the resources.
        /// </summary>
        /// <param name="presentTargetTime">The present target time.</param>
        /// <param name="isContentDirty">The is content dirty.</param>
        public override void PrepareResources(DateTime presentTargetTime, out SharpDX.Bool isContentDirty)
        {
            if (m_surfacePainter == null) { throw new ObjectDisposedException("WP8DrawingSurfaceInterop"); }

            isContentDirty = m_contentDirty;
            m_contentDirty = false;
        }

        private void OnManipulationHostPointerReleased(DrawingSurfaceManipulationHost sender, Windows.UI.Core.PointerEventArgs args)
        {
            if (m_surfacePainter == null) { return; }

        }

        private void OnManipulationHostPointerPressed(DrawingSurfaceManipulationHost sender, Windows.UI.Core.PointerEventArgs args)
        {
            if (m_surfacePainter == null) { return; }

        }

        private void OnManipulationHostPointerMoved(DrawingSurfaceManipulationHost sender, Windows.UI.Core.PointerEventArgs args)
        {
            if (m_surfacePainter == null) { return; }

        }

        /// <summary>
        /// Gets the current frame request count
        /// </summary>
        internal int FrameRequestCountBeforeRender
        {
            get { return m_frameRequestCount; }
        }
    }
}
