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
        private SynchronizationContext m_syncContext;
        #endregion

        #region State
        private bool m_isInFullscreen;
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

        private D3D11.Texture2D m_copyHelperTextureStaging;
        private D3D11.Texture2D m_renderTarget;
        private D3D11.Texture2D m_renderTargetDepth;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepthView;
        #endregion

        /// <summary>
        /// Raises before the render target starts rendering.
        /// </summary>
        public event CancelEventHandler BeforeRender;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullscreenRenderTarget" /> class.
        /// </summary>
        /// <param name="outputInfo">The target output monitor.</param>
        /// <param name="pixelHeight">Height for rendering (pixel height of monitor is taken by default).</param>
        /// <param name="pixelWidth">Width for rendering (pixel width of the monitor is taken by default).</param>
        public FullscreenRenderTarget(EngineOutputInfo outputInfo, int pixelWidth = 0, int pixelHeight = 0)
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
            m_dummyForm.CreateControl();

            // Create and start the renderloop
            m_renderLoop = new RenderLoop(syncContext, this);
            m_renderLoop.Camera.SetScreenSize(pixelWidth, pixelHeight);
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

        Control IInputControlHost.GetWinFormsInputControl()
        {
            return m_dummyForm;
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        void IRenderLoopHost.OnRenderLoop_DisposeViewResources(EngineDevice device)
        {
            m_renderTargetDepthView = GraphicsHelper.DisposeObject(m_renderTargetDepthView);
            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_renderTargetView = GraphicsHelper.DisposeObject(m_renderTargetView);
            m_renderTarget = GraphicsHelper.DisposeObject(m_renderTarget);
            m_copyHelperTextureStaging = GraphicsHelper.DisposeObject(m_copyHelperTextureStaging);
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

            EngineOutputModeInfo outpoutMode = m_targetOutput.SupportedModes.First();

            // Create swapchain and dummy form
            m_swapChain = GraphicsHelper.CreateSwapChainForFullScreen(
                m_dummyForm, 
                m_targetOutput, outpoutMode,
                device, m_renderLoop.ViewConfiguration);

            // Take width and height out of the render target
            m_renderTarget = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTargetView = new D3D11.RenderTargetView(m_device, m_renderTarget);

            //Create the depth buffer
            m_renderTargetDepth = GraphicsHelper.CreateDepthBufferTexture(device, outpoutMode.PixelWidth, outpoutMode.PixelHeight, m_renderLoop.ViewConfiguration);
            m_renderTargetDepthView = new D3D11.DepthStencilView(m_device, m_renderTargetDepth);

            //Define the viewport for rendering
            SharpDX.Mathematics.Interop.RawViewportF viewPort = GraphicsHelper.CreateDefaultViewport(outpoutMode.PixelWidth, outpoutMode.PixelHeight);

            //Return all generated objects
            return Tuple.Create(m_renderTarget, m_renderTargetView, m_renderTargetDepth, m_renderTargetDepthView, viewPort, new Size2(outpoutMode.PixelWidth, outpoutMode.PixelHeight), DpiScaling.Default);
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
                using (var targetOutput = GraphicsCore.Current.HardwareInfo.GetOutputByOutputInfo(m_targetOutput))
                {
                    m_swapChain.SetFullscreenState(
                        new SharpDX.Mathematics.Interop.RawBool(true),
                        targetOutput);
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
