#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using FrozenSky;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Views
{
    //For handling of staging resource see
    // http://msdn.microsoft.com/en-us/library/windows/desktop/ff476259(v=vs.85).aspx
    public class MemoryRenderTarget : IDisposable, IFrozenSkyPainter
    {
        #region Configuration
        private int m_pixelWidth;
        private int m_pixelHeight;
        #endregion

        #region Reference to the render loop
        private RenderLoop m_renderLoop;
        private ThreadSaveQueue<TaskCompletionSource<object>> m_renderAwaitors;
        #endregion

        #region All needed direct3d resources
        private D3D11.Device m_device;
        private D3D11.DeviceContext m_deviceContext;
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
        /// Initializes a new instance of the <see cref="MemoryRenderTarget" /> class.
        /// </summary>
        /// <param name="pixelHeight">Height of the offline render target in pixels.</param>
        /// <param name="pixelWidth">Width of the offline render target in pixels.</param>
        /// <param name="syncContext">Sets the SynchronizationContext which should be used by default.</param>
        public MemoryRenderTarget(int pixelWidth, int pixelHeight, SynchronizationContext syncContext = null)
        {
            //Set confiugration
            m_pixelWidth = pixelWidth;
            m_pixelHeight = pixelHeight;

            m_renderAwaitors = new ThreadSaveQueue<TaskCompletionSource<object>>();
            if (syncContext == null) { syncContext = new SynchronizationContext(); }

            //Create the RenderLoop object
            m_renderLoop = new RenderLoop(
                syncContext,
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);
            m_renderLoop.Camera.SetScreenSize(pixelWidth, pixelHeight);
            m_renderLoop.RegisterRenderLoop();
        }

        /// <summary>
        /// Awaits next render.
        /// </summary>
        public Task AwaitRenderAsync()
        {
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
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources(EngineDevice device)
        {
            m_renderTargetDepthView = GraphicsHelper.DisposeObject(m_renderTargetDepthView);
            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_renderTargetView = GraphicsHelper.DisposeObject(m_renderTargetView);
            m_renderTarget = GraphicsHelper.DisposeObject(m_renderTarget);
            m_copyHelperTextureStaging = GraphicsHelper.DisposeObject(m_copyHelperTextureStaging);

            m_device = null;
            m_deviceContext = null;
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2, DpiScaling> OnRenderLoopCreateViewResources(EngineDevice device)
        {
            int width = m_pixelWidth;
            int height = m_pixelHeight;

            //Get references to current render device
            m_device = device.DeviceD3D11;
            m_deviceContext = m_device.ImmediateContext;

            //Create the swap chain and the render target
            m_renderTarget = GraphicsHelper.CreateRenderTargetTexture(device, width, height, m_renderLoop.ViewConfiguration);
            m_renderTargetView = new D3D11.RenderTargetView(m_device, m_renderTarget);

            //Create the depth buffer
            m_renderTargetDepth = GraphicsHelper.CreateDepthBufferTexture(device, width, height, m_renderLoop.ViewConfiguration);
            m_renderTargetDepthView = new D3D11.DepthStencilView(m_device, m_renderTargetDepth);

            //Define the viewport for rendering
            SharpDX.ViewportF viewPort = GraphicsHelper.CreateDefaultViewport(width, height);

            //Return all generated objects
            return Tuple.Create(m_renderTarget, m_renderTargetView, m_renderTargetDepth, m_renderTargetDepthView, viewPort, new Size2(width, height), DpiScaling.Default);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender(EngineDevice device)
        {
            CancelEventArgs eventArgs = new CancelEventArgs(false);
            if (BeforeRender != null) { BeforeRender(this, eventArgs); }

            return !eventArgs.Cancel;
        }

        private void OnRenderLoopPrepareRendering(EngineDevice device)
        {
            //m_deviceContext.OutputMerger.SetTargets(m_renderTargetDepthView, m_renderTargetView);
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent(EngineDevice device)
        {
            // Finish rendering of all render tasks
            m_deviceContext.Flush();
            m_deviceContext.ClearState();

            // Notify all render awaitors (callers of AwaitRenderAsync method)
            m_renderAwaitors.DequeueAll().ForEachInEnumeration(actAwaitor =>
                {
                    actAwaitor.SetResult(null);
                });
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering(EngineDevice device)
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
    }
}
