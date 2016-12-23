#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Input;
using SeeingSharp.Multimedia.Drawing3D;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.Graphics.Display;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using SDM = SharpDX.Mathematics.Interop;

namespace SeeingSharp.Multimedia.Views
{
    public class SeeingSharpCoreWindowPainter : IDisposable, IInputEnabledView, IRenderLoopHost
    {
        #region Constants
        private double MIN_PIXEL_SIZE_WIDTH = 16.0;
        private double MIN_PIXEL_SIZE_HEIGHT = 16.0;
        #endregion

        #region Configuration
        private CoreWindow m_targetWindow;
        private IDisposable m_observerSizeChanged;
        private DisplayInformation m_displayInfo;
        #endregion

        #region Main engine objects
        private RenderLoop m_renderLoop;
        #endregion

        #region State variables
        private bool m_isDisposed;
        private Size m_lastRefreshTargetSize;
        private DpiScaling m_dpiScaling;
        #endregion

        #region Resources from Direct3D 11
        private DXGI.SwapChain1 m_swapChain;
        private D3D11.Texture2D m_backBuffer;
        //private D3D11.Texture2D m_backBufferMultisampled;
        private D3D11.Texture2D m_depthBuffer;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepth;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpCoreWindowPainter"/> class.
        /// </summary>
        /// <param name="targetWindow">The target window.</param>
        public SeeingSharpCoreWindowPainter(CoreWindow targetWindow)
        {
            // Attach to SizeChanged event (refresh view resources only after a specific time)
            m_targetWindow = targetWindow;
            m_targetWindow.SizeChanged += OnTargetWindow_SizeChanged;
            m_observerSizeChanged = Observable.FromEventPattern<WindowSizeChangedEventArgs>(m_targetWindow, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe((eArgs) => OnTargetWindow_ThrottledSizeChanged(eArgs.Sender as CoreWindow, eArgs.EventArgs));

            // Attach to display setting changes
            m_displayInfo = DisplayInformation.GetForCurrentView();
            m_displayInfo.DpiChanged += OnDisplayInfo_DpiChanged;
            m_displayInfo.OrientationChanged += OnDisplayInfo_OrientationChanged;

            // Store current dpi setting
            m_dpiScaling = new DpiScaling(m_displayInfo.LogicalDpi, m_displayInfo.LogicalDpi);

            // Create the RenderLoop object
            GraphicsCore.Touch();
            m_renderLoop = new Core.RenderLoop(SynchronizationContext.Current, this);
            m_renderLoop.ClearColor = Color4.CornflowerBlue;
            m_renderLoop.CallPresentInUIThread = true;

            m_lastRefreshTargetSize = new Size(0.0, 0.0);

            UpdateRenderLoopViewSize();

            m_renderLoop.RegisterRenderLoop();
        }

        /// <summary>
        /// Stops rendering to the target CoreWindow.
        /// </summary>
        public void Dispose()
        {
            if (m_isDisposed) { return; }
            m_isDisposed = true;

            m_targetWindow.SizeChanged -= OnTargetWindow_SizeChanged;

            m_displayInfo.DpiChanged -= OnDisplayInfo_DpiChanged;
            m_displayInfo.OrientationChanged -= OnDisplayInfo_OrientationChanged;

            // Clear view resources
            m_renderLoop.UnloadViewResources();
            m_renderLoop.DeregisterRenderLoop();
        }

        /// <summary>
        /// Gets the current target pixel size for the render panel.
        /// </summary>
        private Size2 GetTargetRenderPixelSize()
        {
            double currentWidth = m_targetWindow.Bounds.Width * m_dpiScaling.ScaleFactorX;
            double currentHeight = m_targetWindow.Bounds.Height * m_dpiScaling.ScaleFactorY;
            return new Size2(
                (int)(currentWidth > MIN_PIXEL_SIZE_WIDTH ? currentWidth : MIN_PIXEL_SIZE_WIDTH),
                (int)(currentHeight > MIN_PIXEL_SIZE_HEIGHT ? currentHeight : MIN_PIXEL_SIZE_HEIGHT));
        }

        /// <summary>
        /// Update current configured view size on the render loop.
        /// </summary>
        private void UpdateRenderLoopViewSize()
        {
            Size2 viewSize = GetTargetRenderPixelSize();
            m_renderLoop.Camera.SetScreenSize(viewSize.Width, viewSize.Height);
            m_renderLoop.SetCurrentViewSize(
                (int)viewSize.Width,
                (int)viewSize.Height);
        }

        public Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SDM.RawViewportF, Size2, DpiScaling> OnRenderLoop_CreateViewResources(EngineDevice device)
        {
            m_renderLoop.ViewConfiguration.AntialiasingEnabled = false;

            // Get the pixel size of the screen
            Size2 viewSize = GetTargetRenderPixelSize();

            // Create the SwapChain and associate it with the SwapChainBackgroundPanel 
            using (SharpDX.ComObject targetWindowCom = new SharpDX.ComObject(m_targetWindow))
            {
                m_swapChain = GraphicsHelper.CreateSwapChainForCoreWindow(device, targetWindowCom, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            }

            // Get the backbuffer from the SwapChain
            m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTargetView = new D3D11.RenderTargetView(device.DeviceD3D11, m_backBuffer);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(device, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_renderTargetDepth = new D3D11.DepthStencilView(device.DeviceD3D11, m_depthBuffer);

            //Define the viewport for rendering
            SharpDX.Mathematics.Interop.RawViewportF viewPort = GraphicsHelper.CreateDefaultViewport(viewSize.Width, viewSize.Height);
            m_lastRefreshTargetSize = new Size(viewSize.Width, viewSize.Height);

            return Tuple.Create(m_backBuffer, m_renderTargetView, m_depthBuffer, m_renderTargetDepth, viewPort, viewSize, m_dpiScaling);
        }

        public void OnRenderLoop_DisposeViewResources(EngineDevice device)
        {
            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeObject(m_depthBuffer);
            m_renderTargetView = GraphicsHelper.DisposeObject(m_renderTargetView);
            m_backBuffer = GraphicsHelper.DisposeObject(m_backBuffer);
            m_swapChain = GraphicsHelper.DisposeObject(m_swapChain);
        }

        public bool OnRenderLoop_CheckCanRender(EngineDevice device)
        {
            if (m_isDisposed) { return false; }

            return m_targetWindow.Visible;
        }

        public void OnRenderLoop_PrepareRendering(EngineDevice device)
        {

        }

        public void OnRenderLoop_AfterRendering(EngineDevice device)
        {

        }

        public void OnRenderLoop_Present(EngineDevice device)
        {
            if (m_isDisposed) { return; }

            // Present all rendered stuff on screen
            // First parameter indicates synchronization with vertical blank
            //  see http://msdn.microsoft.com/en-us/library/windows/desktop/bb174576(v=vs.85).aspx
            //  see example http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx
            m_swapChain.Present(1, DXGI.PresentFlags.None, new DXGI.PresentParameters());
        }

        private void OnTargetWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            if (!GraphicsCore.IsInitialized) { return; }
            if (m_isDisposed) { return; }

            // Get the current pixel size and apply it on the camera
            Size2 viewSize = GetTargetRenderPixelSize();
            m_renderLoop.Camera.SetScreenSize(viewSize.Width, viewSize.Height);

            //Resize render target only on greater size changes
            double resizeFactorWidth = (double)viewSize.Width > m_lastRefreshTargetSize.Width ? (double)viewSize.Width / m_lastRefreshTargetSize.Width : m_lastRefreshTargetSize.Width / (double)viewSize.Width;
            double resizeFactorHeight = (double)viewSize.Height > m_lastRefreshTargetSize.Height ? (double)viewSize.Height / m_lastRefreshTargetSize.Height : m_lastRefreshTargetSize.Height / (double)viewSize.Height;
            if ((resizeFactorWidth > 1.3) || (resizeFactorHeight > 1.3))
            {
                UpdateRenderLoopViewSize();
            }
        }

        private void OnTargetWindow_ThrottledSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            if (!GraphicsCore.IsInitialized) { return; }
            if (m_isDisposed) { return; }

            // Ignore event, if nothing has changed..
            Size2 actSize = GetTargetRenderPixelSize();
            if ((m_lastRefreshTargetSize.Width == (int)actSize.Width) &&
                (m_lastRefreshTargetSize.Height == (int)actSize.Height))
            {
                return;
            }

            UpdateRenderLoopViewSize();
        }

        private void OnDisplayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            if (m_isDisposed) { return; }
        }

        private void OnDisplayInfo_DpiChanged(DisplayInformation sender, object args)
        {
            if (m_isDisposed) { return; }

            m_dpiScaling = new DpiScaling(m_displayInfo.LogicalDpi, m_displayInfo.LogicalDpi);

            this.UpdateRenderLoopViewSize();
        }

        /// <summary>
        /// Gets or sets the clear color for the 3D view.
        /// </summary>
        public Windows.UI.Color ClearColor
        {
            get { return m_renderLoop.ClearColor.ToWindowsColor(); }
            set { m_renderLoop.ClearColor = new Color4(value); }
        }

        /// <summary>
        /// Does the target control have focus?
        /// (Return true here if rendering runs, because in winrt we are everytime at fullscreen)
        /// </summary>
        public bool Focused
        {
            get { return m_renderLoop.IsRegisteredOnMainLoop; }
        }

        /// <summary>
        /// Gets the current 3D scene.
        /// </summary>
        public Scene Scene
        {
            get
            {
                if (m_renderLoop.Scene == null) { return m_renderLoop.TargetScene; }
                else { return m_renderLoop.Scene; }
            }
            set
            {
                m_renderLoop.SetScene(value);
            }
        }

        /// <summary>
        /// Gets or sets the current 3D camera.
        /// </summary>
        public Camera3DBase Camera
        {
            get { return m_renderLoop.Camera; }
            set { m_renderLoop.Camera = value; }
        }

        public Size2 PixelSize
        {
            get { return GetTargetRenderPixelSize(); }
        }

        public Size2 ActualSize
        {
            get
            {
                return new Size2((int)m_targetWindow.Bounds.Width, (int)m_targetWindow.Bounds.Height);
            }
        }

        /// <summary>
        /// Gets a collection containing all SceneComponents associated to this view.
        /// </summary>
        public ObservableCollection<SceneComponentBase> SceneComponents
        {
            get { return m_renderLoop.SceneComponents; }
        }

        /// <summary>
        /// True if the control is connected with the main rendering loop.
        /// False if something went wrong.
        /// </summary>
        public bool IsOperational
        {
            get
            {
                return m_renderLoop.IsOperational;
            }
        }

        public CoreWindow TargetWindow
        {
            get { return m_targetWindow; }
        }

        public CoreDispatcher Disptacher
        {
            get
            {
                if (m_targetWindow == null) { return null; }
                return m_targetWindow.Dispatcher;
            }
        }

        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }
    }
}
