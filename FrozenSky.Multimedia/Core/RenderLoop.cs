#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using FrozenSky.Multimedia.Drawing2D;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Infrastructure;
using FrozenSky.Util;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D2D = SharpDX.Direct2D1;
#if DESKTOP
using GDI = System.Drawing;
using WinForms = System.Windows.Forms;
#endif

namespace FrozenSky.Multimedia.Core
{
    public class RenderLoop : IDisposable
    {
        internal const int MIN_VIEW_WIDTH = 32;
        internal const int MIN_VIEW_HEIGHT = 32;

        // Configuration values
        private SynchronizationContext m_guiSyncContext;
        private GraphicsViewConfiguration m_viewConfiguration;
        private bool m_discardRendering;
        private Color4 m_clearColor;
        private ICamera3D m_camera;

        // Async actions
        private ThreadSaveQueue<Action> m_afterPresentActions;

        // Target parameters for rendering
        private DateTime m_lastTargetParametersChanged;
        private EngineDevice m_targetDevice;
        private Size2 m_targetSize;
        private DpiScaling m_currentDpiScaling;
        private Scene m_targetScene;

        // Callback methods for current host object
        private Func<EngineDevice, Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2, DpiScaling>> m_actionCreateViewResources;
        private Action<EngineDevice> m_actionDisposeViewResources;
        private Func<EngineDevice, bool> m_actionCheckCanRender;
        private Action<EngineDevice> m_actionPrepareRendering;
        private Action<EngineDevice> m_actionAfterRendering;
        private Action<EngineDevice> m_actionPresent;

        // Values needed for runtime
        private bool m_lastRenderSuccessfully;
        private bool m_nextRenderAllowed;
        private int m_totalRenderCount;
        private RenderState m_renderState;

        // Direct3D resources and other values gathered during graphics loading
        private List<Custom2DDrawingLayer> m_2dDrawingLayers;
        private DebugDrawingLayer m_debugDrawingLayer;
        private EngineDevice m_currentDevice;
        private Size2 m_currentViewSize;
        private Size2F m_currentViewSizeDpiScaled;
        private Scene m_currentScene;
#if !WINDOWS_PHONE
        private Direct2DOverlayRenderer m_d2dOverlay;
#endif
        private D3D11.Texture2D m_renderTarget;
        private D3D11.Texture2D m_renderTargetDepth;
        private D3D11.RenderTargetView m_renderTargetView;
        private D3D11.DepthStencilView m_renderTargetDepthView;
        private SharpDX.ViewportF m_viewport;

        //Direct3D resources for rendertarget capturing
        // A staging texture for reading contents by Cpu
        // A standard texture for copying data from multisample texture to standard one
        // see http://www.rolandk.de/wp/2013/06/inhalt-der-rendertarget-textur-in-ein-bitmap-kopieren/
        private D3D11.Texture2D m_copyHelperTextureStaging;
        private D3D11.Texture2D m_copyHelperTextureStandard;

        // Filters
        private List<SceneObjectFilter> m_filters;

        //Other resources
        private ViewInformation m_viewInformation;

        /// <summary>
        /// Raised when the corresponding device has changed.
        /// </summary>
        public event EventHandler DeviceChanged;

        /// <summary>
        /// Raised when the current camera has changed.
        /// </summary>
        public event EventHandler CameraChanged;

        /// <summary>
        /// Raised when it is possible for the UI thread to manipulate current filter list.
        /// </summary>
        public event EventHandler<ManipulateFilterListArgs> ManipulateFilterList;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderLoop" /> class.
        /// </summary>
        internal RenderLoop(
            SynchronizationContext guiSyncContext,
            Func<EngineDevice, Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2, DpiScaling>> actionCreateViewResources,
            Action<EngineDevice> actionDisposeViewResources,
            Func<EngineDevice, bool> actionCheckCanRender,
            Action<EngineDevice> actionPrepareRendering,
            Action<EngineDevice> actionAfterRendering,
            Action<EngineDevice> actionPresent)
        {
            m_afterPresentActions = new ThreadSaveQueue<Action>();

            m_guiSyncContext = guiSyncContext;

            m_filters = new List<SceneObjectFilter>();
            m_2dDrawingLayers = new List<Custom2DDrawingLayer>();

            // Load DebugDrawingLayer if debug mode is enabled
            if (GraphicsCore.IsInitialized &&
                GraphicsCore.Current.IsDebugEnabled)
            {
                m_debugDrawingLayer = new DebugDrawingLayer();
            }

            m_viewInformation = new ViewInformation(this);
            m_viewConfiguration = new GraphicsViewConfiguration();

            m_afterPresentActions = new ThreadSaveQueue<Action>();

            // Assign all given actions
            m_actionCreateViewResources = actionCreateViewResources;
            m_actionDisposeViewResources = actionDisposeViewResources;
            m_actionCheckCanRender = actionCheckCanRender;
            m_actionPrepareRendering = actionPrepareRendering;
            m_actionAfterRendering = actionAfterRendering;
            m_actionPresent = actionPresent;

            // Create default objects
            m_clearColor = Color4.White;

            // Set initial target parameters
            m_lastTargetParametersChanged = DateTime.MinValue;
            m_targetDevice = null;
            m_targetSize = new Size2(0, 0);

            if (!GraphicsCore.IsInitialized) { return; }

            // Apply default rendering device for this RenderLoop
            this.SetRenderingDevice(GraphicsCore.Current.DefaultDevice);

            // Create default scene and camera
            this.Camera = new PerspectiveCamera3D();
            this.SetScene(new Scene());
        }

        /// <summary>
        /// Sets the current with and height of the view.
        /// The RenderLoop will apply the values later.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal void SetCurrentViewSize(int width, int height)
        {
            if (width < MIN_VIEW_WIDTH) { width = MIN_VIEW_WIDTH; }
            if (height < MIN_VIEW_HEIGHT) { height = MIN_VIEW_HEIGHT; }

            m_targetSize = new Size2(width, height);
        }

        /// <summary>
        /// Gets the current index of this view within the current scene.
        /// </summary>
        public int GetViewIndex()
        {
            ViewInformation viewInformation = m_viewInformation;
            if (viewInformation != null) { return viewInformation.ViewIndex + 1; }
            else { return -1; }
        }

        /// <summary>
        /// Sets the given rendering device for this renderloop.
        /// </summary>
        /// <param name="device">The device to be set.</param>
        public void SetRenderingDevice(EngineDevice device)
        {
            // Only set state values here. All logic is triggered during render process
            m_targetDevice = device;
        }

        /// <summary>
        /// Sets the scene for rendering
        /// </summary>
        /// <param name="scene">The scene to be set.</param>
        public void SetScene(Scene scene)
        {
            m_targetScene = scene;
        }

        /// <summary>
        /// Forces the view to reload itself.
        /// </summary>
        public void ForceViewReload()
        {
            m_viewConfiguration.ViewNeedsRefresh = true;
        }

        /// <summary>
        /// Registers the given drawing layer that is used for 2d rendering.
        /// </summary>
        /// <param name="drawingLayer">The layer to be registered.</param>
        public Task Register2DDrawingLayerAsync(Custom2DDrawingLayer drawingLayer)
        {
            TaskCompletionSource<object> result = new TaskCompletionSource<object>();

            m_afterPresentActions.Enqueue(() =>
            {
                try
                {
                    if (!m_2dDrawingLayers.Contains(drawingLayer))
                    {
                        m_2dDrawingLayers.Add(drawingLayer);

                        result.SetResult(null);
                    }
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            });

            return result.Task;
        }

        /// <summary>
        /// Deregisters the given drawing layer from this RenderLoop.
        /// </summary>
        /// <param name="drawingLayer">The drawing layer to be deregistered.</param>
        public Task Deregister2DDrawingLayerAsync(Custom2DDrawingLayer drawingLayer)
        {
            TaskCompletionSource<object> result = new TaskCompletionSource<object>();

            m_afterPresentActions.Enqueue(() =>
            {
                try
                {
                    m_2dDrawingLayers.Remove(drawingLayer);
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            });

            return result.Task;
        }

        /// <summary>
        /// Gets the object on the given pixel location (location local to this control).
        /// </summary>
        public async Task<List<SceneObject>> PickObjectAsync(Point localLocation, PickingOptions pickingOptions)
        {
            List<SceneObject> result = null;

            if ((m_currentScene != null) && 
                (m_camera != null))
            {
                Matrix projectionMatrix = m_camera.Projection;

                Vector3 pickingVector;
                pickingVector.X = (((2.0f * localLocation.X) / m_currentViewSize.Width) - 1) / projectionMatrix.M11;
                pickingVector.Y = -(((2.0f * localLocation.Y) / m_currentViewSize.Height) - 1) / projectionMatrix.M22;
                pickingVector.Z = 1f;

                Matrix worldMatrix = Matrix.Identity;
                Matrix viewWorld = m_camera.View * worldMatrix;
                Matrix inversionViewWorld = Matrix.Invert(viewWorld); 

                Vector3 rayDirection = new Vector3(
                    pickingVector.X * inversionViewWorld.M11 + pickingVector.Y * inversionViewWorld.M21 + pickingVector.Z * inversionViewWorld.M31,
                    pickingVector.X * inversionViewWorld.M12 + pickingVector.Y * inversionViewWorld.M22 + pickingVector.Z * inversionViewWorld.M32,
                    pickingVector.X * inversionViewWorld.M13 + pickingVector.Y * inversionViewWorld.M23 + pickingVector.Z * inversionViewWorld.M33);
                Vector3 rayStart = new Vector3(
                    inversionViewWorld.M41,
                    inversionViewWorld.M42,
                    inversionViewWorld.M43);

                // Pick objects async in update thread
                await m_currentScene.PerformBesideRenderingAsync(() =>
                {
                    if ((m_currentScene != null) &&
                        (m_viewInformation != null) &&
                        (m_currentScene.IsViewRegistered(m_viewInformation)))
                    {
                        result = m_currentScene.Pick(rayStart, rayDirection, m_viewInformation, pickingOptions);
                    }
                    else
                    {
                        result = new List<SceneObject>();
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// Changes the ViewConfiguration object.
        /// </summary>
        /// <param name="newViewConfiguration">The new configuration object to be applied.</param>
        public Task ExchangeViewConfigurationAsync(GraphicsViewConfiguration newViewConfiguration)
        {
            TaskCompletionSource<object> result = new TaskCompletionSource<object>();

            m_afterPresentActions.Enqueue(() =>
            {
                var deviceConfig = m_viewConfiguration.DeviceConfiguration;

                m_viewConfiguration = newViewConfiguration;
                m_viewConfiguration.DeviceConfiguration = deviceConfig;
                m_viewConfiguration.ViewNeedsRefresh = true;

                result.TrySetResult(null);
            });

            return result.Task;
        }

#if DESKTOP
        /// <summary>
        /// Takes a screenshot and returns it as a gdi bitmap.
        /// </summary>
        public Task<GDI.Bitmap> GetScreenshotGdiAsync()
        {
            TaskCompletionSource<GDI.Bitmap> result = new TaskCompletionSource<GDI.Bitmap>();

            m_afterPresentActions.Enqueue(() =>
            {
                try
                {
                    GDI.Bitmap resultBitmap = GetScreenshotGdiInternal();

                    result.SetResult(resultBitmap);
                }
                catch (Exception ex)
                {
                    result.SetException(ex);
                }
            });

            return result.Task;
        }

        /// <summary>
        /// Gets a screenshot in form of a gdi bitmap.
        /// (be careful. This call is executed synchronous.
        /// </summary>
        internal GDI.Bitmap GetScreenshotGdiInternal()
        {
            // Concept behind this see http://www.rolandk.de/wp/2013/06/inhalt-der-rendertarget-textur-in-ein-bitmap-kopieren/
            int width = m_currentViewSize.Width;
            int height = m_currentViewSize.Height;
            if (width <= 0) { throw new InvalidOperationException("View not initialized correctly!"); }
            if (height <= 0) { throw new InvalidOperationException("View not initialized correctly!"); }

            //Get and read data from the gpu (create copy helper texture on demand)
            if (m_copyHelperTextureStaging == null)
            {
                m_copyHelperTextureStaging = GraphicsHelper.CreateStagingTexture(m_currentDevice, width, height);
                m_copyHelperTextureStandard = GraphicsHelper.CreateTexture(m_currentDevice, width, height);
            }
            m_currentDevice.DeviceImmediateContextD3D11.ResolveSubresource(m_renderTarget, 0, m_copyHelperTextureStandard, 0, GraphicsHelper.DEFAULT_TEXTURE_FORMAT);
            m_currentDevice.DeviceImmediateContextD3D11.CopyResource(m_copyHelperTextureStandard, m_copyHelperTextureStaging);

            // Load the bitmap
            GDI.Bitmap resultBitmap = GraphicsHelper.LoadBitmapFromStagingTexture(m_currentDevice, m_copyHelperTextureStaging, width, height);
            return resultBitmap;
        }
#endif

        /// <summary>
        /// Resets the render counter to zero.
        /// </summary>
        public void ResetRenderCounter()
        {
            m_totalRenderCount = 0;
        }

        /// <summary>
        /// Refreshes the view resources.
        /// </summary>
        private void RefreshViewResources()
        {
            if (m_currentDevice == null) { return; }

            // Unload current view resources first
            this.UnloadViewResources();

            // Recreate view resources
            var generatedViewResources = m_actionCreateViewResources(m_currentDevice);
            if (generatedViewResources == null) { return; }
            m_renderTarget = generatedViewResources.Item1;
            m_renderTargetView = generatedViewResources.Item2;
            m_renderTargetDepth = generatedViewResources.Item3;
            m_renderTargetDepthView = generatedViewResources.Item4;
            m_viewport = generatedViewResources.Item5;
            m_currentViewSize = generatedViewResources.Item6;
            m_targetSize = m_currentViewSize;
            m_currentDpiScaling = generatedViewResources.Item7;
            m_currentViewSizeDpiScaled = new Size2F(
                (float)m_currentViewSize.Width / m_currentDpiScaling.ScaleFactorX,
                (float)m_currentViewSize.Height / m_currentDpiScaling.ScaleFactorY);

            m_viewConfiguration.ViewNeedsRefresh = false;

            // Try to create a Direct2D overlay
            m_d2dOverlay = new Direct2DOverlayRenderer(
                m_currentDevice,
                m_renderTarget,
                m_currentViewSize.Width, m_currentViewSize.Height,
                m_currentDpiScaling);

            // Create or update current renderstate
            if ((m_renderState == null) ||
                (m_renderState.Device != m_currentDevice))
            {
                m_renderState = new RenderState(
                    m_currentDevice, 
                    GraphicsCore.Current.PerformanceCalculator,
                    new RenderTargets(m_renderTargetView, m_renderTargetDepthView),
                    m_viewport, m_camera, m_viewInformation);
            }
        }

        /// <summary>
        /// Unload all loaded ViewResources.
        /// This call ensures that it will be executed in the correct UI thread.
        /// </summary>
        internal async Task UnloadViewResourcesAsync()
        {
            await m_guiSyncContext.PostAsync(() =>
            {
                this.UnloadViewResources();
            });
        }

        /// <summary>
        /// Unloads all view resources (is also called from EngineMainLoop in a synchronized execution block).
        /// </summary>
        internal void UnloadViewResources()
        {
            // TODO: Do this action synchronized..

            m_lastRenderSuccessfully = false;

            if (m_currentDevice == null) { return; }

            // Dispose resources of parent object first
            m_actionDisposeViewResources(m_currentDevice);

#if !WINDOWS_PHONE
            // Free direct2d render target (if created)
            GraphicsHelper.SafeDispose(ref m_d2dOverlay);
#endif

            // Set values to initial
            m_renderTarget = null;
            m_renderTargetView = null;
            m_renderTargetDepth = null;
            m_renderTargetDepthView = null;
            m_viewport = new SharpDX.ViewportF();
            m_currentViewSize = new Size2(MIN_VIEW_WIDTH, MIN_VIEW_HEIGHT);

            // Dispose local resources
            GraphicsHelper.SafeDispose(ref m_copyHelperTextureStaging);
            GraphicsHelper.SafeDispose(ref m_copyHelperTextureStandard);
        }

        /// <summary>
        /// Internal method that resets some flags directly before rendering.
        /// </summary>
        internal void ResetFlagsBeforeRendering()
        {
            // Handle VisibleObjectCount field
            this.VisibleObjectCount = this.VisibleObjectCountInternal;
            this.VisibleObjectCountInternal = 0;
        }

        /// <summary>
        /// Prepares rendering (refreshes view resources, post last rendered image to the view, ...)
        /// </summary>
        internal async Task<List<Action>> PrepareRenderAsync()
        {
            List<Action> continuationActions = new List<Action>();
            if (m_discardRendering) { return continuationActions; }

            // Call present from Threadpool (if configured)
            if(!CallPresentInUIThread)
            {
                PresentFrameInternal();
                m_lastRenderSuccessfully = false;
            }

            await m_guiSyncContext.PostAsync(() =>
            {
                // Call present from UI thread (if configured)
                if (CallPresentInUIThread)
                {
                    PresentFrameInternal();
                    m_lastRenderSuccessfully = false;
                }

                if (m_discardRendering) { return; }

                // Update view frustum
                m_viewInformation.UpdateFrustum(m_camera.ViewProjection);

                // Allow next rendering by default
                m_nextRenderAllowed = true;

                // Need to update view parameters?              (=> Later: Render Thread)
                if ((m_renderTargetView == null) ||
                    (m_targetSize != m_currentViewSize) ||
                    ((m_targetDevice != null) && (m_targetDevice != m_currentDevice)) ||
                    (m_viewConfiguration.ViewNeedsRefresh))
                {
                    try
                    {
                        // Trigger deregister on scene if needed
                        bool reregisterOnScene = (m_targetDevice != null) && (m_targetDevice != m_currentDevice) && (m_currentScene != null);
                        if (reregisterOnScene)
                        {
                            Scene localScene = m_currentScene;
                            continuationActions.Add(() => localScene.DeregisterView(m_viewInformation));
                        }

                        // Unload view resources first
                        UnloadViewResources();

                        // Update device ob object (size is updated in RefreshViewResources)
                        if (m_targetDevice != null)
                        {
                            m_currentDevice = m_targetDevice;
                            m_targetDevice = null;

                            DeviceChanged.Raise(this, EventArgs.Empty);
                        }

                        // Load view resources again
                        RefreshViewResources();

                        // Trigger reregister on scene if needed
                        if (reregisterOnScene)
                        {
                            Scene localScene = m_currentScene;
                            continuationActions.Add(() => localScene.RegisterView(m_viewInformation));
                        }
                    }
                    catch (Exception ex)
                    {
                        m_nextRenderAllowed = false;

                        throw new FrozenSkyGraphicsException("Unable to refresh view on device " + m_currentDevice + "!", ex);
                    }
                }

                // Check needed resources
                if ((m_currentDevice == null) ||
                    (m_renderTargetView == null) ||
                    (m_renderTargetDepthView == null))
                {
                    m_nextRenderAllowed = false;
                    return;
                }

                // Handle changed scene
                if ((m_targetScene != null) &&
                    (m_currentScene != m_targetScene))
                {
                    try
                    {
                        // Trigger deregister on the current scene
                        if (m_currentScene != null)
                        {
                            Scene localScene = m_currentScene;
                            continuationActions.Add(() => localScene.DeregisterView(m_viewInformation));
                        }

                        // Change scene property
                        m_currentScene = m_targetScene;
                        m_targetScene = null;

                        // Trigger reregister on the new scene
                        if (m_currentScene != null)
                        {
                            Scene localScene = m_currentScene;
                            continuationActions.Add(() => localScene.RegisterView(m_viewInformation));
                        }
                    }
                    catch (Exception ex)
                    {
                        m_nextRenderAllowed = false;

                        throw new FrozenSkyGraphicsException("Unable to change the scene!", ex);
                    }
                }
                // Ensure that this loop is registered on the current view
                else if((m_currentScene != null) && 
                        (!m_currentScene.IsViewRegistered(m_viewInformation)))
                {
                    Scene localScene = m_currentScene;
                    continuationActions.Add(() => localScene.RegisterView(m_viewInformation));
                }

                // Check needed resources
                if (m_currentScene == null)
                {
                    m_nextRenderAllowed = false;
                    return;
                }

                try
                {
                    // Check here wether we can render or not
                    if (!m_actionCheckCanRender(m_currentDevice))
                    {
                        m_nextRenderAllowed = false;
                        return;
                    }

                    // Perform some preparation for rendering
                    m_actionPrepareRendering(m_currentDevice);
                }
                catch (Exception ex)
                {
                    m_nextRenderAllowed = false;
                    throw new FrozenSkyGraphicsException("Unable to prepare rendering", ex);
                }

                // Let UI manipulate current filter list
                this.ManipulateFilterList.Raise(this, new ManipulateFilterListArgs(m_filters));
            });

            return continuationActions;
        }

        /// <summary>
        /// Presents the current frame.
        /// </summary>
        private void PresentFrameInternal()
        {
            // Post last frame to the screen if rendering was successful.
            //  For now: Do this in render thread. This should be possible
            //  see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb205075(v=vs.85).aspx#Multithread_Considerations
            if (m_lastRenderSuccessfully)
            {
                try
                {
                    // Presents all contents on the screen
                    GraphicsCore.Current.ExecuteAndMeasureActivityDuration(
                        string.Format(Constants.PERF_RENDERLOOP_PRESENT, m_currentDevice.DeviceIndex, m_viewInformation.ViewIndex + 1),
                        () => m_actionPresent(m_currentDevice));

                    // Execute all deferred actions to be called after present
                    m_afterPresentActions.DequeueAll().ForEachInEnumeration((actAction) => actAction());

                    // Finish rendering now
                    m_actionAfterRendering(m_currentDevice);
                }
                catch (Exception ex)
                {
                    throw new FrozenSkyGraphicsException("Unable to present a view with device " + m_currentDevice + "!", ex);
                }
            }
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        internal void Render()
        {
            if (m_discardRendering) { return; }
            if (!m_nextRenderAllowed) { return; }
            m_nextRenderAllowed = false;

            var renderTimeMeasurenment = GraphicsCore.Current.BeginMeasureActivityDuration(
                string.Format(Constants.PERF_RENDERLOOP_RENDER, m_currentDevice.DeviceIndex, m_viewInformation.ViewIndex + 1));
            try
            {
                // Set default rastarization state
                D3D11.RasterizerState rasterState = null;
                bool isWireframeEnabled = m_viewConfiguration.WireframeEnabled;
                if (isWireframeEnabled)
                {
                    rasterState = new D3D11.RasterizerState(m_currentDevice.DeviceD3D11, new D3D11.RasterizerStateDescription()
                    {
                        CullMode = D3D11.CullMode.Back,
                        FillMode = D3D11.FillMode.Wireframe,
                        IsFrontCounterClockwise = false,
                        DepthBias = 0,
                        SlopeScaledDepthBias = 0f,
                        DepthBiasClamp = 0f,
                        IsDepthClipEnabled = true,
                        IsAntialiasedLineEnabled = false,
                        IsMultisampleEnabled = false,
                        IsScissorEnabled = false
                    });
                    m_currentDevice.DeviceImmediateContextD3D11.Rasterizer.State = rasterState;
                }

                // Update render state
                m_renderState.Reset(
                    new RenderTargets(m_renderTargetView, m_renderTargetDepthView),
                    m_viewport, m_camera, m_viewInformation);
                m_renderState.ApplyMaterial(null);

                // Paint using Direct3D
                m_currentDevice.DeviceImmediateContextD3D11.ClearRenderTargetView(m_renderTargetView, this.ClearColor.ToDXColor());
                m_currentDevice.DeviceImmediateContextD3D11.ClearDepthStencilView(m_renderTargetDepthView, D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1f, 0);

                // Render currently configured scene
                if ((m_currentScene != null) && 
                    (m_camera != null) && 
                    (m_currentScene.IsViewRegistered(m_viewInformation)))
                {
                    // Renders current scene on this view
                    m_currentScene.Render(m_renderState);
                }

                // Clear current state after rendering
                m_renderState.ClearState();

                // Handle wireframe state
                if (isWireframeEnabled)
                {
                    m_currentDevice.DeviceImmediateContextD3D11.Rasterizer.State = null;
                    rasterState.Dispose();
                }

                if (m_totalRenderCount < Int32.MaxValue) { m_totalRenderCount++; }

                // Render 2D overlay if possible (may be not available on some older OS or older graphics cards)
                if ((m_viewConfiguration.Overlay2DEnabled) &&
                    (m_d2dOverlay != null) &&
                    (m_d2dOverlay.IsLoaded))
                {
                    var d2dOverlayTime = GraphicsCore.Current.PerformanceCalculator.BeginMeasureActivityDuration(
                        string.Format(Constants.PERF_RENDERLOOP_RENDER_2D, m_currentDevice.DeviceIndex, m_viewInformation.ViewIndex + 1));
                    m_d2dOverlay.BeginDraw(m_renderState);
                    try
                    {
                        m_renderState.RenderTarget2D = m_d2dOverlay.RenderTarget2D;
                        m_renderState.Graphics2D = m_d2dOverlay.Graphics;

                        // Render scene contents
                        m_currentScene.Render2DOverlay(m_renderState);

                        // Perform rendering of custom 2D drawing layers
                        foreach (Custom2DDrawingLayer act2DLayer in m_2dDrawingLayers)
                        {
                            act2DLayer.Draw2DInternal(m_d2dOverlay.Graphics);
                        }

                        // Draw debug layer if created
                        if (m_debugDrawingLayer != null)
                        {
                            m_debugDrawingLayer.Draw2DInternal(m_d2dOverlay.Graphics);
                        }
                    }
                    finally
                    {
                        m_renderState.RenderTarget2D = null;
                        m_renderState.Graphics2D = null;

                        m_d2dOverlay.EndDraw(m_renderState);
                        d2dOverlayTime.Dispose();
                    }
                }

                // Send all draw calls to the device and wait until finished
                m_currentDevice.DeviceImmediateContextD3D11.Flush();

                // Update flag indicating that last render was successful
                m_lastRenderSuccessfully = true;
            }
            finally
            {
                renderTimeMeasurenment.Dispose();
            }
        }

        /// <summary>
        /// Deregisters this view from the render loop.
        /// </summary>
        public void DeregisterRenderLoop()
        {
            // Deregister this Renderloop object
            if (IsRegisteredOnMainLoop)
            {
                EngineMainLoop.Current.DeregisterRenderLoop(this);
            }
        }

        /// <summary>
        /// Registers this view on the render loop.
        /// </summary>
        public void RegisterRenderLoop()
        {
            // Deregister this Renderloop object
            if (!IsRegisteredOnMainLoop)
            {
                EngineMainLoop.Current.RegisterRenderLoop(this);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Deregister this Renderloop object
            EngineMainLoop.Current.DeregisterRenderLoop(this);

            GraphicsHelper.SafeDispose(ref m_debugDrawingLayer);
        }

        /// <summary>
        /// Gets an identifyer related to this render looop.
        /// </summary>
        public ViewInformation ViewInformation
        {
            get { return m_viewInformation; }
        }

        /// <summary>
        /// Gets the current view configuration.
        /// </summary>
        public GraphicsViewConfiguration ViewConfiguration
        {
            get { return m_viewConfiguration; }
        }

        /// <summary>
        /// Gets the current scene object.
        /// </summary>
        public Scene Scene
        {
            get 
            {
                if (m_targetScene != null) { return m_targetScene; }
                return m_currentScene; 
            }
        }

        /// <summary>
        /// Gets the current target scene.
        /// </summary>
        internal Scene TargetScene
        {
            get { return m_targetScene; }
        }

        public int TotalRenderCount
        {
            get { return m_totalRenderCount; }
        }

        /// <summary>
        /// Are view resources loaded?
        /// </summary>
        public bool ViewResourcesLoaded
        {
            get { return m_renderTarget != null; }
        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        public bool DiscardRendering
        {
            get { return m_discardRendering; }
            set { m_discardRendering = value; }
        }

        /// <summary>
        /// Gets the current SynchronizationContext.
        /// </summary>
        public SynchronizationContext UISynchronizationContext
        {
            get { return m_guiSyncContext; }
        }

        /// <summary>
        /// Gets or sets the current clear color.
        /// </summary>
        public Color4 ClearColor
        {
            get { return m_clearColor; }
            set { m_clearColor = value; }
        }

        /// <summary>
        /// Gets the collection containing all filters.
        /// </summary>
        internal List<SceneObjectFilter> Filters
        {
            get { return m_filters; }
        }

        /// <summary>
        /// Gets or sets current camera object.
        /// </summary>
        public ICamera3D Camera
        {
            get { return m_camera; }
            set
            {
                if (m_camera != value)
                {
                    if (value == null) { m_camera = new PerspectiveCamera3D(); }
                    else { m_camera = value; }

                    // Apply current screen size on the camera
                    m_camera.SetScreenSize(this.CurrentViewSize.Width, this.CurrentViewSize.Height);

                    CameraChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the current view size.
        /// </summary>
        public Size2 CurrentViewSize
        {
            get { return m_currentViewSize; }
        }

        /// <summary>
        /// Gets the device this renderloop is using.
        /// </summary>
        public EngineDevice Device
        {
            get { return m_currentDevice; }
        }

        //public bool CyclicRendering
        //{
        //    get { return m_renderTimer.Enabled; }
        //    set { m_renderTimer.Enabled = value; }
        //}

        /// <summary>
        /// Gets the total count of visible objects.
        /// </summary>
        public int VisibleObjectCount
        {
            get;
            private set;
        }

        public bool IsAttachedToVisibleView
        {
            get { return IsRegisteredOnMainLoop; }
        }

        /// <summary>
        /// Internal field that is used to count visible objects.
        /// </summary>
        internal int VisibleObjectCountInternal;

        /// <summary>
        /// Is this RenderLoop registered on the main loop?
        /// </summary>
        internal bool IsRegisteredOnMainLoop;

        internal bool CallPresentInUIThread;
    }
}