using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Phone.Graphics.Interop;
using Windows.Phone.Input.Interop;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace FrozenSky.Multimedia.Views
{
    /// <summary>
    /// This class encapsulates all logic needed for rendering on a Windows Phone DrawingSurface control.
    /// More information: http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj207012(v=vs.105).aspx
    /// </summary>
    public class FrozenSkyDrawingSurfacePainter
    {
        private int MIN_PIXEL_SIZE_WIDTH = 10;
        private int MIN_PIXEL_SIZE_HEIGHT = 10;

        private DrawingSurface m_targetPanel;
        private RenderLoop m_renderLoop;
        private WP8DrawingSurfaceInterop m_drawingInterop;

        private Size2 m_lastRefreshTargetSize;
        private IDisposable m_observerSizeChanged;
        private bool m_detachOnUnload;

        private D3D11.Texture2D m_backBufferSynchronizing;
        private D3D11.Texture2D m_backBuffer;
        private D3D11.Texture2D m_depthBuffer;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyDrawingSurfacePainter"/> class.
        /// </summary>
        public FrozenSkyDrawingSurfacePainter()
        {
            // Create the RenderLoop object
            m_renderLoop = new Core.RenderLoop(
                SynchronizationContext.Current,
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);
            m_renderLoop.ClearColor = Color4.CornflowerBlue;
            m_renderLoop.CallPresentInUIThread = false;

            m_detachOnUnload = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyDrawingSurfacePainter"/> class.
        /// </summary>
        /// <param name="targetPanel">Attaches to the given target panel.</param>
        public FrozenSkyDrawingSurfacePainter(DrawingSurface targetPanel)
            : this()
        {
            this.Attach(targetPanel);
        }

        /// <summary>
        /// Attaches the the painter to the given target panel.
        /// </summary>
        /// <param name="targetPanel">The target panel.</param>
        /// <exception cref="System.InvalidOperationException">Unable to attach to new SwapChainBackgroundPanel: Renderer is already attached to another one!</exception>
        public void Attach(DrawingSurface targetPanel)
        {
            if (m_targetPanel != null) { throw new InvalidOperationException("Unable to attach to new SwapChainBackgroundPanel: Renderer is already attached to another one!"); }

            // Set default values
            m_lastRefreshTargetSize = new Size2(0, 0);
            m_targetPanel = targetPanel;

            // Attach to SizeChanged event (refresh view resources only after a specific time)
            m_observerSizeChanged = Observable.FromEventPattern<SizeChangedEventArgs>(m_targetPanel, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe((eArgs) => OnTargetPanelThrottledSizeChanged(eArgs.Sender, eArgs.EventArgs));

            m_targetPanel.SizeChanged += OnTargetPanelSizeChanged;
            m_targetPanel.Loaded += OnTargetPanelLoaded;
            m_targetPanel.Unloaded += OnTargetPanelUnloaded;

            // Hook-up native component to DrawingSurface
            m_drawingInterop = new WP8DrawingSurfaceInterop(this);
            m_targetPanel.SetContentProvider(m_drawingInterop);
            //m_targetPanel.SetManipulationHandler(m_drawingInterop);

            // Define unloading behavior
            if (VisualTreeHelper.GetParent(m_targetPanel) != null)
            {
                m_renderLoop.RegisterRenderLoop();
            }

            //// Register simple mouse movement
            //InitializeMouseMovement();
        }

        public void Detach()
        {
            try
            {
                if (m_targetPanel == null) { return; }

                //// Deregister simple mouse movement
                //UnloadMouseMovement();

                // Clear view resources
                m_renderLoop.UnloadViewResources();
                m_renderLoop.DeregisterRenderLoop();

                // Reset interop settings
                m_targetPanel.SetContentProvider(null);
                m_targetPanel.SetManipulationHandler(null);
                m_drawingInterop = null;

                // Clear event registrations
                m_targetPanel.SizeChanged -= OnTargetPanelSizeChanged;
                m_targetPanel.Loaded -= OnTargetPanelLoaded;
                m_targetPanel.Unloaded -= OnTargetPanelUnloaded;

                //Clear created references
                m_observerSizeChanged.Dispose();
                m_observerSizeChanged = null;
                m_targetPanel = null;
            }
            catch (Exception ex)
            {
                CommonTools.RaiseUnhandledException(
                    this.GetType(), this, ex,
                    "Detaching the FrozenSyBackgroundPanelPainter");
            }
        }

        /// <summary>
        /// Gets the current size of the render target.
        /// </summary>
        internal Size2 GetCurrentRenderTargetSize()
        {
            return m_lastRefreshTargetSize;
        }

        /// <summary>
        /// Gets the true render size for this view.
        /// </summary>
        private Size2 GetTargetRenderPixelSize()
        {
            if (m_targetPanel == null) { return new Size2(MIN_PIXEL_SIZE_WIDTH, MIN_PIXEL_SIZE_HEIGHT); }

            Size2 result = new Size2(
                (int)Math.Floor(m_targetPanel.ActualWidth * Application.Current.Host.Content.ScaleFactor / 100.0f + 0.5f),
                (int)Math.Floor(m_targetPanel.ActualHeight * Application.Current.Host.Content.ScaleFactor / 100.0f + 0.5f));
            result.Width = Math.Max(result.Width, MIN_PIXEL_SIZE_WIDTH);
            result.Height = Math.Max(result.Height, MIN_PIXEL_SIZE_HEIGHT);
            return result;
        }

        /// <summary>
        /// Called when the size of the target control has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnTargetPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!GraphicsCore.IsInitialized) { return; }

            // Get the current pixel size and apply it on the camera
            Size2 viewSize = GetTargetRenderPixelSize();
            m_renderLoop.Camera.SetScreenSize(viewSize.Width, viewSize.Height);

            //Resize render target only on greater size changes
            double resizeFactorWidth = (double)viewSize.Width > m_lastRefreshTargetSize.Width ? (double)viewSize.Width / m_lastRefreshTargetSize.Width : m_lastRefreshTargetSize.Width / (double)viewSize.Width;
            double resizeFactorHeight = (double)viewSize.Height > m_lastRefreshTargetSize.Height ? (double)viewSize.Height / m_lastRefreshTargetSize.Height : m_lastRefreshTargetSize.Height / (double)viewSize.Height;
            if ((resizeFactorWidth > 1.3) || (resizeFactorHeight > 1.3))
            {
                m_renderLoop.SetCurrentViewSize(viewSize.Width, viewSize.Height);
            }
        }

        /// <summary>
        /// Called when the size of the target control has changed (throttled)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnTargetPanelThrottledSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (!GraphicsCore.IsInitialized) { return; }

                Size2 viewSize = GetTargetRenderPixelSize();

                //Ignore event, if nothing has changed..
                if (m_lastRefreshTargetSize == viewSize) { return; }

                m_renderLoop.Camera.SetScreenSize(viewSize.Width, viewSize.Height);
                m_renderLoop.SetCurrentViewSize(
                    (int)viewSize.Width,
                    (int)viewSize.Height);
            }
            catch (Exception ex)
            {
                CommonTools.RaiseUnhandledException(this.GetType(), this, ex);
            }
        }

        /// <summary>
        /// Called when the target panel is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTargetPanelLoaded(object sender, RoutedEventArgs e)
        {
            m_renderLoop.RegisterRenderLoop();
        }

        /// <summary>
        /// Called when the target panel is unloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTargetPanelUnloaded(object sender, RoutedEventArgs e)
        {
            m_renderLoop.DeregisterRenderLoop();

            // Trigger detach if requested
            if (m_detachOnUnload)
            {
                Detach();
            }
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources(EngineDevice engineDevice)
        {
            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeObject(m_depthBuffer);
            m_renderTargetView = GraphicsHelper.DisposeObject(m_renderTargetView);
            m_backBuffer = GraphicsHelper.DisposeObject(m_backBuffer);
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2> OnRenderLoopCreateViewResources(EngineDevice engineDevice)
        {
            Size2 viewSize = GetTargetRenderPixelSize();

            // Create the backbuffer
            m_backBuffer = GraphicsHelper.CreateRenderTargetTexture(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_backBufferSynchronizing = GraphicsHelper.CreateRenderTargetTexture(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_renderTargetView = new D3D11.RenderTargetView(engineDevice.DeviceD3D11, m_backBuffer);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_renderTargetDepth = new D3D11.DepthStencilView(engineDevice.DeviceD3D11, m_depthBuffer);

            //Define the viewport for rendering
            SharpDX.ViewportF viewPort = GraphicsHelper.CreateDefaultViewport(viewSize.Width, viewSize.Height);
            m_lastRefreshTargetSize = viewSize;

            return Tuple.Create(m_backBuffer, m_renderTargetView, m_depthBuffer, m_renderTargetDepth, viewPort, viewSize);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender(EngineDevice engineDevice)
        {
            if (m_targetPanel == null) { return false; }
            if (m_targetPanel.ActualWidth <= 0) { return false; }
            if (m_targetPanel.ActualHeight <= 0) { return false; }
            if (m_drawingInterop == null) { return false; }
            if (m_drawingInterop.FrameRequestCountBeforeRender > 0) { return false; }

            return true;
        }

        /// <summary>
        /// Prepares rendering.
        /// </summary>
        /// <param name="engineDevice">Current graphics device.</param>
        private void OnRenderLoopPrepareRendering(EngineDevice engineDevice)
        {

        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent(EngineDevice engineDevice)
        {
            D3D11.DeviceContext deviceContext = engineDevice.DeviceImmediateContextD3D11;

            // Copy all contents of the render target to the texture that is used for synchronization
            deviceContext.Flush();
            deviceContext.CopyResource(
                m_backBuffer,
                m_backBufferSynchronizing);

            // Request next frame
            m_drawingInterop.RequestNextFrame();
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering(EngineDevice engineDevice)
        {

        }

        /// <summary>
        /// Gets the renderloop of this painter.
        /// </summary>
        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        /// <summary>
        /// Gets the scene of this painter.
        /// </summary>
        public Scene Scene
        {
            get { return m_renderLoop.Scene; }
        }

        public ICamera3D Camera
        {
            get { return m_renderLoop.Camera; }
        }

        internal D3D11.Texture2D RenderTargetForSynchronization
        {
            get
            {
                return m_backBufferSynchronizing;
            }
        }
    }
}
