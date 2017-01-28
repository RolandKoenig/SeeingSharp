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
using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Input;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Views
{
    //For handling of staging resource see
    // http://msdn.microsoft.com/en-us/library/windows/desktop/ff476259(v=vs.85).aspx
    public class FullscreenRenderTarget 
        : IDisposable, ISeeingSharpPainter, IRenderLoopHost, IInputEnabledView, IInputControlHost
    {
        #region Configuration
        private EngineOutputInfo m_targetOutput;
        private EngineOutputModeInfo m_targetOutputMode;
        private SynchronizationContext m_syncContext;
        #endregion

        #region State
        private bool m_isInFullscreen;
        private bool m_checkFullscreenNextTime;
        #endregion

        #region Reference to the render loop
        private RenderLoop m_renderLoop;
        private ThreadSaveQueue<TaskCompletionSource<object>> m_renderAwaitors;
        #endregion

        #region All needed direct3d resources
        private DummyForm m_dummyForm;
        private D3D11.Device m_device;
        private D3D11.DeviceContext m_deviceContext;
        private DXGI.SwapChain1 m_swapChain;
        private D3D11.Texture2D m_renderTarget;
        private D3D11.Texture2D m_renderTargetDepth;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepthView;
        #endregion

        /// <summary>
        /// Raises before the render target starts rendering.
        /// </summary>
        public event CancelEventHandler BeforeRender;

        public event EventHandler WindowDestroyed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullscreenRenderTarget" /> class.
        /// </summary>
        /// <param name="outputInfo">The target output monitor.</param>
        /// <param name="initialMode">The initial view mode.</param>
        public FullscreenRenderTarget(EngineOutputInfo outputInfo, EngineOutputModeInfo initialMode = default(EngineOutputModeInfo))
        {
            outputInfo.EnsureNotNull(nameof(outputInfo));

            // Check whether we are inside a win.forms application
            System.Windows.Forms.WindowsFormsSynchronizationContext syncContext = SynchronizationContext.Current
                as System.Windows.Forms.WindowsFormsSynchronizationContext;
            if (syncContext == null)
            {
                throw new SeeingSharpException($"{nameof(FullscreenRenderTarget)} muss be created inside a Windows.Forms application context!");
            }
            m_syncContext = syncContext;

            //Set confiugration
            m_targetOutput = outputInfo;
            m_targetOutputMode = initialMode;
            if(m_targetOutputMode.HostOutput != outputInfo) { m_targetOutputMode = m_targetOutput.DefaultMode; }
            m_renderAwaitors = new ThreadSaveQueue<TaskCompletionSource<object>>();

            // Ensure that graphics is initialized
            GraphicsCore.Touch();

            // Create the dummy form
            m_dummyForm = new DummyForm();
            m_dummyForm.SetStyleCustom(ControlStyles.AllPaintingInWmPaint, true);
            m_dummyForm.SetStyleCustom(ControlStyles.ResizeRedraw, true);
            m_dummyForm.SetStyleCustom(ControlStyles.OptimizedDoubleBuffer, false);
            m_dummyForm.SetStyleCustom(ControlStyles.Opaque, true);
            m_dummyForm.SetStyleCustom(ControlStyles.Selectable, true);
            m_dummyForm.SetDoubleBuffered(false);
            m_dummyForm.MouseEnter += (sender, eArgs) => Cursor.Hide();
            m_dummyForm.MouseLeave += (sender, eArgs) => Cursor.Show();
            m_dummyForm.HandleDestroyed += OnDummyForm_HandleDestroyed;
            m_dummyForm.GotFocus += OnDummyForm_GotFocus;
            m_dummyForm.CreateControl();

            // Create and start the renderloop
            m_renderLoop = new RenderLoop(syncContext, this);
            m_renderLoop.Camera.SetScreenSize(m_targetOutputMode.PixelWidth, m_targetOutputMode.PixelHeight);
            m_renderLoop.RegisterRenderLoop();
        }

        /// <summary>
        /// Awaits next render.
        /// </summary>
        public Task AwaitRenderAsync()
        {
            if (!this.IsOperational) { return Task.Delay(100); }

            TaskCompletionSource<object> result = new TaskCompletionSource<object>();
            m_renderAwaitors.Enqueue(result);

            return result.Task;
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            m_renderLoop.Dispose();
        }

        /// <summary>
        /// Changes the output mode to the given one.
        /// </summary>
        /// <param name="newMode">The new mode.</param>
        public void ChangeOutputMode(EngineOutputModeInfo newMode)
        {
            newMode.HostOutput.EnsureEqual(m_targetOutput, $"{nameof(newMode)}.{nameof(newMode.HostOutput)}");

            m_targetOutputMode = newMode;
            m_renderLoop.SetCurrentViewSize(m_targetOutputMode.PixelWidth, m_targetOutputMode.PixelHeight);
            m_renderLoop.ForceViewRefresh();
        }

        Control IInputControlHost.GetWinFormsInputControl()
        {
            return m_dummyForm;
        }

        private void OnDummyForm_HandleDestroyed(object sender, EventArgs e)
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if (m_renderLoop.IsRegisteredOnMainLoop)
            {
                m_renderLoop.DiscardRendering = true;
                m_renderLoop.DeregisterRenderLoop();
            }

            this.WindowDestroyed.Raise(this, EventArgs.Empty);
        }

        private void OnDummyForm_GotFocus(object sender, EventArgs e)
        {
            m_checkFullscreenNextTime = true;
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        void IRenderLoopHost.OnRenderLoop_DisposeViewResources(EngineDevice device)
        {
            // Switch to fullscreen
            if (m_isInFullscreen)
            {
                m_isInFullscreen = false;
                m_swapChain.SetFullscreenState(false, null);
            }

            m_renderTargetDepthView = GraphicsHelper.DisposeObject(m_renderTargetDepthView);
            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_renderTargetView = GraphicsHelper.DisposeObject(m_renderTargetView);
            m_renderTarget = GraphicsHelper.DisposeObject(m_renderTarget);
            m_swapChain = GraphicsHelper.DisposeObject(m_swapChain);

            m_device = null;
            m_deviceContext = null;
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.Mathematics.Interop.RawViewportF, Size2, DpiScaling> IRenderLoopHost.OnRenderLoop_CreateViewResources(EngineDevice device)
        {
            //Get references to current render device
            m_device = device.DeviceD3D11;
            m_deviceContext = m_device.ImmediateContext;

            // Create swapchain and dummy form
            m_swapChain = GraphicsHelper.CreateSwapChainForFullScreen(
                m_dummyForm, 
                m_targetOutput, m_targetOutputMode,
                device, m_renderLoop.ViewConfiguration);

            // Take width and height out of the render target
            m_renderTarget = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTargetView = new D3D11.RenderTargetView(m_device, m_renderTarget);

            //Create the depth buffer
            m_renderTargetDepth = GraphicsHelper.CreateDepthBufferTexture(device, m_targetOutputMode.PixelWidth, m_targetOutputMode.PixelHeight, m_renderLoop.ViewConfiguration);
            m_renderTargetDepthView = new D3D11.DepthStencilView(m_device, m_renderTargetDepth);

            //Define the viewport for rendering
            SharpDX.Mathematics.Interop.RawViewportF viewPort = GraphicsHelper.CreateDefaultViewport(m_targetOutputMode.PixelWidth, m_targetOutputMode.PixelHeight);

            //Return all generated objects
            return Tuple.Create(m_renderTarget, m_renderTargetView, m_renderTargetDepth, m_renderTargetDepthView, viewPort, new Size2(m_targetOutputMode.PixelWidth, m_targetOutputMode.PixelHeight), DpiScaling.Default);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        bool IRenderLoopHost.OnRenderLoop_CheckCanRender(EngineDevice device)
        {
            CancelEventArgs eventArgs = new CancelEventArgs(false);
            if (BeforeRender != null) { BeforeRender(this, eventArgs); }

            return !eventArgs.Cancel;
        }

        void IRenderLoopHost.OnRenderLoop_PrepareRendering(EngineDevice device)
        {
            // Start showing the target form
            if(!m_dummyForm.Visible)
            {
                m_dummyForm.Show();
            }

            // Switch to fullscreen
            if (!m_isInFullscreen)
            {
                m_isInFullscreen = true;
                m_checkFullscreenNextTime = false;
                using (var targetOutput = GraphicsCore.Current.HardwareInfo.GetOutputByOutputInfo(m_targetOutput))
                {
                    m_swapChain.SetFullscreenState(
                        new SharpDX.Mathematics.Interop.RawBool(true),
                        targetOutput);
                    m_isInFullscreen = true;
                }
            }
            else if(m_checkFullscreenNextTime)
            {
                m_checkFullscreenNextTime = false;

                SharpDX.Mathematics.Interop.RawBool isFullscreen = false;
                DXGI.Output fullscreenOutput = null;
                m_swapChain.GetFullscreenState(out isFullscreen, out fullscreenOutput);
                if (isFullscreen == false)
                {
                    m_isInFullscreen = false;

                    if ((m_dummyForm.IsHandleCreated) &&
                       (m_dummyForm.Focused || m_dummyForm.ContainsFocus))
                    {
                        m_isInFullscreen = true;
                        using (var targetOutput = GraphicsCore.Current.HardwareInfo.GetOutputByOutputInfo(m_targetOutput))
                        {
                            m_swapChain.SetFullscreenState(
                                new SharpDX.Mathematics.Interop.RawBool(true),
                                targetOutput);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        void IRenderLoopHost.OnRenderLoop_Present(EngineDevice device)
        {
            //Present all rendered stuff on screen
            m_swapChain.Present(0, DXGI.PresentFlags.DoNotWait, new DXGI.PresentParameters());

            // Notify all render awaitors (callers of AwaitRenderAsync method)
            m_renderAwaitors.DequeueAll().ForEachInEnumeration(actAwaitor =>
                {
                    actAwaitor.SetResult(null);
                });
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        void IRenderLoopHost.OnRenderLoop_AfterRendering(EngineDevice device)
        {

        }

        /// <summary>
        /// Gets or sets the scene.
        /// </summary>
        public Scene Scene
        {
            get { return m_renderLoop.Scene; }
            set { m_renderLoop.SetScene(value); }
        }

        public Camera3DBase Camera
        {
            get { return m_renderLoop.Camera; }
            set { m_renderLoop.Camera = value; }
        }

        public Color4 ClearColor
        {
            get { return m_renderLoop.ClearColor; }
            set { m_renderLoop.ClearColor = value; }
        }

        public SynchronizationContext UISynchronizationContext
        {
            get { return m_renderLoop.UISynchronizationContext; }
        }

        /// <summary>
        /// Gets the renderloop object.
        /// </summary>
        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        public bool IsOperational
        {
            get { return m_renderLoop.IsOperational; }
        }

        public bool Focused
        {
            get
            {
                return (m_dummyForm != null) && (m_dummyForm.ContainsFocus);
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class DummyForm : Form
        {
            public void SetStyleCustom(ControlStyles flag, bool value)
            {
                base.SetStyle(flag, value);
            }

            public void SetDoubleBuffered(bool value)
            {
                base.DoubleBuffered = value;
            }
        }
    }
}
