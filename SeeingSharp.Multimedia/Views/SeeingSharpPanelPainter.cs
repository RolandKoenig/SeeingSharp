#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using System;
using System.Reactive.Linq;
using System.Threading;
using System.Collections.Generic;
using SharpDX;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Input;
using SeeingSharp.Util;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using SeeingSharp.Multimedia.Drawing2D;

namespace SeeingSharp.Multimedia.Views
{
    // Using SwapChainBackgroundPanel to render to the background of the WinRT app
    //  see http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx
    public partial class SeeingSharpPanelPainter : ISeeingSharpPainter, IDisposable, IInputEnabledView
    {
        private double MIN_PIXEL_SIZE_WIDTH = 100.0;
        private double MIN_PIXEL_SIZE_HEIGHT = 100.0;

        #region Global members
        private RenderLoop m_renderLoop;
        private bool m_detachOnUnload;
        #endregion

        #region SwapChainBackgroundPanel local members
        private SwapChainPanelWrapper m_targetPanel;
        private float m_currentDpi;
        private Size m_lastRefreshTargetSize;
        private IDisposable m_observerSizeChanged;
        #endregion

        #region Members for input handling
        private List<ISeeingSharpInputHandler> m_inputHandlers;
        private SeeingSharpInputMode m_inputMode;
        #endregion

        #region Resources from Direct3D 11
        private DXGI.SwapChain1 m_swapChain;
        private D3D11.Texture2D m_backBuffer;
        private D3D11.Texture2D m_backBufferMultisampled;
        private D3D11.Texture2D m_depthBuffer;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepth;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpPanelPainter" /> class.
        /// </summary>
        /// <param name="targetPanel">The target panel.</param>
        public SeeingSharpPanelPainter()
        {
            m_inputHandlers = new List<ISeeingSharpInputHandler>();

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
        /// Initializes a new instance of the <see cref="SeeingSharpPanelPainter"/> class.
        /// </summary>
        /// <param name="targetPanel">The target panel.</param>
        public SeeingSharpPanelPainter(SwapChainBackgroundPanel targetPanel)
            : this()
        {
            this.Attach(targetPanel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpPanelPainter"/> class.
        /// </summary>
        /// <param name="targetPanel">The target panel.</param>
        public SeeingSharpPanelPainter(SwapChainPanel targetPanel)
            : this()
        {
            this.Attach(targetPanel);
        }

        /// <summary>
        /// Attaches the renderer to the given SwapChainBackgroundPanel.
        /// </summary>
        /// <param name="targetPanel">The target panel to attach to.</param>
        public void Attach(SwapChainPanel targetPanel)
        {
            this.Attach(new SwapChainPanelWrapper(targetPanel));
        }

        /// <summary>
        /// Attaches the renderer to the given SwapChainBackgroundPanel.
        /// </summary>
        /// <param name="targetPanel">The target panel to attach to.</param>
        public void Attach(SwapChainBackgroundPanel targetPanel)
        {
            this.Attach(new SwapChainPanelWrapper(targetPanel));
        }

        /// <summary>
        /// Detaches the renderer from the current target panel.
        /// </summary>
        public void Detach()
        {
            try
            {
                if (m_targetPanel == null) { return; }

                // Clear view resources
                m_renderLoop.UnloadViewResources();
                m_renderLoop.DeregisterRenderLoop();

                // Updates currently active input handlers
                InputHandlerFactory.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, true);

                // Clear event registrations
                m_targetPanel.SizeChanged -= OnTargetPanelSizeChanged;
                m_targetPanel.Loaded -= OnTargetPanelLoaded;
                m_targetPanel.Unloaded -= OnTargetPanelUnloaded;

                //Clear created references
                m_observerSizeChanged.Dispose();
                m_observerSizeChanged = null;
                m_targetPanel.Dispose();
                m_targetPanel = null;
            }
            catch(Exception ex)
            {
                CommonTools.RaiseUnhandledException(
                    this.GetType(), this, ex,
                    "Detaching the FrozenSyBackgroundPanelPainter");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Detach();
        }

        /// <summary>
        /// Attaches the renderer to the given SwapChainBackgroundPanel.
        /// </summary>
        /// <param name="targetPanel">The target panel to attach to.</param>
        private void Attach(SwapChainPanelWrapper targetPanel)
        {
            if (m_targetPanel != null) { throw new InvalidOperationException("Unable to attach to new SwapChainBackgroundPanel: Renderer is already attached to another one!"); }

            // Set default values
            m_lastRefreshTargetSize = new Size(0.0, 0.0);
            m_currentDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            m_targetPanel = targetPanel;

            // Attach to SizeChanged event (refresh view resources only after a specific time)
            m_observerSizeChanged = Observable.FromEventPattern<SizeChangedEventArgs>(m_targetPanel, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe((eArgs) => OnTargetPanelThrottledSizeChanged(eArgs.Sender, eArgs.EventArgs));

            m_targetPanel.SizeChanged += OnTargetPanelSizeChanged;
            m_targetPanel.Loaded += OnTargetPanelLoaded;
            m_targetPanel.Unloaded += OnTargetPanelUnloaded;

            UpdateRenderLoopViewSize();

            // Define unloading behavior
            if (VisualTreeHelper.GetParent(m_targetPanel.Panel) != null)
            {
                m_renderLoop.RegisterRenderLoop();

                // Updates currently active input handlers
                InputHandlerFactory.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, false);
            }
        }

        /// <summary>
        /// Gets the current target pixel size for the render panel.
        /// </summary>
        private Size2 GetTargetRenderPixelSize()
        {
            double currentWidth = this.PixelWidth;
            double currentHeight = this.PixelHeight;

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

        /// <summary>
        /// Called when the target panel gets unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTargetPanelUnloaded(object sender, RoutedEventArgs e)
        {
            m_renderLoop.DeregisterRenderLoop();

            // Updates currently active input handlers
            InputHandlerFactory.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, true);

            // Trigger detach if requested
            if(m_detachOnUnload)
            {
                Detach();
            }
        }

        /// <summary>
        /// Called when the target panel gets loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTargetPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (!m_renderLoop.IsRegisteredOnMainLoop)
            {
                m_renderLoop.RegisterRenderLoop();

                // Updates currently active input handlers
                InputHandlerFactory.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, false);
            }
        }

        /// <summary>
        /// Called when the size of the target panel has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.SizeChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnTargetPanelThrottledSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (!GraphicsCore.IsInitialized) { return; }

                //Ignore event, if nothing has changed..
                if (m_lastRefreshTargetSize == m_targetPanel.RenderSize) { return; }

                UpdateRenderLoopViewSize();
            }
            catch (Exception ex)
            {
                CommonTools.RaiseUnhandledException(this.GetType(), this, ex);
            }
        }

        /// <summary>
        /// Called when the size of the target panel has changed.
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
                UpdateRenderLoopViewSize();
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
            m_backBufferMultisampled = GraphicsHelper.DisposeObject(m_backBufferMultisampled);
            m_swapChain = GraphicsHelper.DisposeObject(m_swapChain);
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2, DpiScaling> OnRenderLoopCreateViewResources(EngineDevice engineDevice)
        {
            m_backBufferMultisampled = null;

            Size2 viewSize = GetTargetRenderPixelSize();

            // Create the SwapChain and associate it with the SwapChainBackgroundPanel 
            m_swapChain = GraphicsHelper.CreateSwapChainForComposition(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_targetPanel.SwapChain = m_swapChain;

            // Get the backbuffer from the SwapChain
            m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);

            // Define the render target (in case of multisample an own render target)
            D3D11.Texture2D backBufferForRenderloop = null;
            if (m_renderLoop.ViewConfiguration.AntialiasingEnabled)
            {
                m_backBufferMultisampled = GraphicsHelper.CreateRenderTargetTexture(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
                m_renderTargetView = new D3D11.RenderTargetView(engineDevice.DeviceD3D11, m_backBufferMultisampled);
                backBufferForRenderloop = m_backBufferMultisampled;
            }
            else
            {
                m_renderTargetView = new D3D11.RenderTargetView(engineDevice.DeviceD3D11, m_backBuffer);
                backBufferForRenderloop = m_backBuffer;
            }

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(engineDevice, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            m_renderTargetDepth = new D3D11.DepthStencilView(engineDevice.DeviceD3D11, m_depthBuffer);

            //Define the viewport for rendering
            SharpDX.ViewportF viewPort = GraphicsHelper.CreateDefaultViewport(viewSize.Width, viewSize.Height);
            m_lastRefreshTargetSize = m_targetPanel.RenderSize;

            DpiScaling dpiScaling = new DpiScaling();
            dpiScaling.DpiX = m_currentDpi;
            dpiScaling.DpiY = m_currentDpi;

            return Tuple.Create(backBufferForRenderloop, m_renderTargetView, m_depthBuffer, m_renderTargetDepth, viewPort, viewSize, dpiScaling);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender(EngineDevice engineDevice)
        {
            if (m_targetPanel == null) { return false; }
            if (m_targetPanel.ActualWidth <= 0) { return false; }
            if (m_targetPanel.ActualHeight <= 0) { return false; }

            return true;
        }

        private void OnRenderLoopPrepareRendering(EngineDevice engineDevice)
        {
            if ((m_renderLoop != null) &&
                (m_renderLoop.Camera != null))
            {
                // Update movement for all running input handlers
                foreach (var actInputHandler in m_inputHandlers)
                {
                    actInputHandler.UpdateMovement();
                }
            }
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent(EngineDevice engineDevice)
        {
            // Copy contents of the backbuffer if in multisampling mode
            if (m_backBufferMultisampled != null)
            {
                engineDevice.DeviceImmediateContextD3D11.ResolveSubresource(m_backBufferMultisampled, 0, m_backBuffer, 0, GraphicsHelper.DEFAULT_TEXTURE_FORMAT);
            }

            // Present all rendered stuff on screen
            // First parameter indicates synchronization with vertical blank
            //  see http://msdn.microsoft.com/en-us/library/windows/desktop/bb174576(v=vs.85).aspx
            //  see example http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx
            m_swapChain.Present(1, DXGI.PresentFlags.None);
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering(EngineDevice engineDevice)
        {

        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        public bool DiscardRendering
        {
            get { return m_renderLoop.DiscardRendering; }
            set { m_renderLoop.DiscardRendering = value; }
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

        /// <summary>
        /// Gets current renderloop object.
        /// </summary>
        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        /// <summary>
        /// Should we detach automatically if the TargetPanel gets unloaded?
        /// </summary>
        public bool DetachOnUnload
        {
            get { return m_detachOnUnload; }
            set { m_detachOnUnload = value; }
        }

        /// <summary>
        /// Is this painter attached to any panel?
        /// </summary>
        public bool IsAttachedToGui
        {
            get { return m_targetPanel != null; }
        }

        /// <summary>
        /// Width of the swap chain to create or resize.
        /// </summary>
        public int PixelWidth
        {
            get
            {
                return (int)(m_targetPanel.RenderSize.Width * m_currentDpi / 96.0);
            }
        }

        /// <summary>
        /// Height of the swap chain to create or resize.
        /// </summary>
        public int PixelHeight
        {
            get
            {
                return (int)(m_targetPanel.RenderSize.Height * m_currentDpi / 96.0);
            }
        }

        /// <summary>
        /// Gets or sets the clear color for the 3D view.
        /// </summary>
        public Windows.UI.Color ClearColor
        {
            get
            {
                return m_renderLoop.ClearColor.ToWindowsColor();
            }
            set
            {
                m_renderLoop.ClearColor = new Color4(
                    value.R, value.G, value.B, value.A);
            }
        }

        /// <summary>
        /// Does the target control have focus?
        /// (Return true here if rendering runs, because in winrt we are everytime at fullscreen)
        /// </summary>
        bool IInputEnabledView.Focused
        {
            get { return m_renderLoop.IsRegisteredOnMainLoop; }
        }

        public Panel TargetPanel
        {
            get 
            {
                if (m_targetPanel == null) { return null; }
                return m_targetPanel.Panel; 
            }
        }

        public SeeingSharpInputMode InputMode
        {
            get { return m_inputMode; }
            set
            {
                if((m_inputMode != value) &&
                   (m_inputHandlers.Count > 0))
                {
                    m_inputMode = value;
                    InputHandlerFactory.UpdateInputHandlerList(
                        this, m_inputHandlers, m_renderLoop, false);
                }
                else
                {
                    m_inputMode = value;
                }
            }
        }
    }
}