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

using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Multimedia.Input;
using FrozenSky.Util;
using FrozenSky.Infrastructure;
using FrozenSky;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Linq;

//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using GDI = System.Drawing;

namespace FrozenSky.Multimedia.Views
{
    public partial class FrozenSkyRendererControl : Control, IFrozenSkyPainter, IInputEnabledView
    {
        private const string TEXT_GRAPHICS_NOT_INITIALIZED = "Graphics not initialized!";

        // Main reference to 3D-Engine
        private RenderLoop m_renderLoop;

        // Resources for Direct3D 11
        private DXGI.Factory m_factory;
        private DXGI.SwapChain m_swapChain;
        private D3D11.Device m_renderDevice;
        private D3D11.DeviceContext m_renderDeviceContext;
        private D3D11.RenderTargetView m_renderTarget;
        private D3D11.DepthStencilView m_renderTargetDepth;
        private D3D11.Texture2D m_backBuffer;
        private D3D11.Texture2D m_depthBuffer;

        // Members for input handling
        private List<IFrozenSkyFreeCameraInputHandler> m_inputHandlers;

        // Generic members
        private Brush m_backBrush;
        private Brush m_foreBrushText;
        private Brush m_backBrushText;
        private Pen m_borderPen;

        private bool m_isMouseInside;

        /// <summary>
        /// Raised when it is possible for the UI thread to manipulate current filter list.
        /// </summary>
        [Category("Graphics")]
        public event EventHandler<ManipulateFilterListArgs> ManipulateFilterList;

        /// <summary>
        /// Raises when mouse was down a short amount of time.
        /// </summary>
        [Category("Graphics")]
        public event EventHandler<MouseEventArgs> MouseClickEx;

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct3D11Panel"/> class.
        /// </summary>
        public FrozenSkyRendererControl()
        {
            m_inputHandlers = new List<IFrozenSkyFreeCameraInputHandler>();

            //Set style parameters for this control
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            base.SetStyle(ControlStyles.Opaque, true);
            base.SetStyle(ControlStyles.Selectable, true);
            base.DoubleBuffered = false;

            //Create the render loop
            m_renderLoop = new RenderLoop(
                SynchronizationContext.Current,
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);
            m_renderLoop.CameraChanged += OnRenderLoopCameraChanged;
            m_renderLoop.ManipulateFilterList += OnRenderLoopManipulateFilterList;
            m_renderLoop.ClearColor = new Color4(this.BackColor);
            m_renderLoop.DiscardRendering = true;
            this.Disposed += (sender, eArgs) =>
            {
                m_renderLoop.Dispose();
            };

            if (FrozenSkyApplication.IsInitialized)
            {
                m_renderLoop.SetScene(new Scene());
                m_renderLoop.Camera = new PerspectiveCamera3D();

                // Updates currently active input handlers
                InputHandlerContainer.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, false);

                //Observe resize event and throttle them
                this.HandleCreateDisposeOnControl(
                    () => Observable.FromEventPattern(this, "Resize")
                        .Merge(Observable.FromEventPattern(m_renderLoop.ViewConfiguration, "ConfigurationChanged"))
                        .Throttle(TimeSpan.FromSeconds(0.5))
                        .ObserveOn(SynchronizationContext.Current)
                        .Subscribe((eArgs) => OnThrottledViewRecreation()));

                //Initialize background brush
                UpdateDrawingResourcesForFailoverRendering();

                // Observe mouse click event
                this.HandleCreateDisposeOnControl(() =>
                {
                    var mouseDownEvent = Observable.FromEventPattern<MouseEventArgs>(this, "MouseDown");
                    var mouseUpEvent = Observable.FromEventPattern<MouseEventArgs>(this, "MouseUp");
                    var mouseClick = from down in mouseDownEvent
                                     let timeStampDown = DateTime.UtcNow
                                     from up in mouseUpEvent
                                     where up.EventArgs.Button == down.EventArgs.Button
                                     let timeStampUp = DateTime.UtcNow
                                     where timeStampUp - timeStampDown < TimeSpan.FromMilliseconds(200.0)
                                     select new { Down = down, Up = up };
                    return mouseClick.Subscribe((givenItem) =>
                    {
                        MouseClickEx.Raise(this, givenItem.Up.EventArgs);
                    });
                });
            }

            this.Disposed += OnDisposed;

            UpdateDrawingResourcesForFailoverRendering();
        }

        /// <summary>
        /// Gets the scene object below the cursor.
        /// </summary>
        public async Task<SceneObject> GetObjectBelowCursorAsync()
        {
            if (!m_isMouseInside) { return null; }

            List<SceneObject> objects = await m_renderLoop.PickObjectAsync(
                Point.FromGdiPoint(this.PointToClient(Cursor.Position)),
                new PickingOptions() { OnlyCheckBoundingBoxes = false });

            if (objects == null) { return null; }
            return objects.FirstOrDefault();
        }

        /// <summary>
        /// Gets all objects that are below the cursor.
        /// </summary>
        public async Task<List<SceneObject>> GetObjectsBelowCursorAsync()
        {
            if (!m_isMouseInside) { return new List<SceneObject>(); }

            return await m_renderLoop.PickObjectAsync(
                Point.FromGdiPoint(this.PointToClient(Cursor.Position)),
                new PickingOptions() { OnlyCheckBoundingBoxes = false });
        }

        /// <summary>
        /// Saves a screenshot to harddisc.
        /// </summary>
        /// <param name="targetFile">Target file path.</param>
        public async Task SaveScreenshotAsync(string targetFile)
        {
            if (m_backBuffer != null)
            {
                Bitmap screenshot = await m_renderLoop.GetScreenshotGdiAsync();
                screenshot.Save(targetFile);
            }
        }

        /// <summary>
        /// Saves a screenshot to harddisc.
        /// </summary>
        /// <param name="targetFile">Target file path.</param>
        /// <param name="fileFormat">Target file format.</param>
        public async Task SaveScreenshotAsync(string targetFile, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            if (m_backBuffer != null)
            {
                Bitmap screenshot = await m_renderLoop.GetScreenshotGdiAsync();
                screenshot.Save(targetFile, imageFormat);
            }
        }

        /// <summary>
        /// Called when system wants to paint this panel.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if((!m_renderLoop.ViewResourcesLoaded) ||
               (!m_renderLoop.IsRegisteredOnMainLoop))
            {
                // Paint using System.Drawing
                e.Graphics.FillRectangle(m_backBrush, e.ClipRectangle);

                // Paint a simple grid on the background to have something for the Designer
                if (!GraphicsCore.IsInitialized)
                {
                    GDI.SizeF targetSize = e.Graphics.MeasureString(TEXT_GRAPHICS_NOT_INITIALIZED, this.Font);
                    GDI.RectangleF targetRect = new GDI.RectangleF(
                        10f, 10f, targetSize.Width, targetSize.Height);
                    if ((targetRect.Width > 10) &&
                       (targetRect.Height > 10))
                    {
                        e.Graphics.FillRectangle(m_backBrushText, targetRect);
                        e.Graphics.DrawString(
                            TEXT_GRAPHICS_NOT_INITIALIZED, this.Font,
                            m_foreBrushText, targetRect.X, targetRect.Y);
                    }
                }

                // Paint a border rectangle
                e.Graphics.DrawRectangle(
                    m_borderPen,
                    new GDI.Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }

        /// <summary>
        /// Updates the movement.
        /// </summary>
        private void UpdateMovement()
        {
            // Update movement for all running input handlers
            foreach(var actInputHandler in m_inputHandlers)
            {
                actInputHandler.UpdateMovement();
            }
        }

        /// <summary>
        /// Stops rendering.
        /// </summary>
        private void StartRendering()
        {
            if (this.DesignMode) { return; }
            if (!FrozenSkyApplication.IsInitialized) { return; }

            if (!m_renderLoop.IsRegisteredOnMainLoop)
            {
                m_renderLoop.SetCurrentViewSize(this.Width, this.Height);
                m_renderLoop.DiscardRendering = false;
                m_renderLoop.RegisterRenderLoop();

                // Updates currently active input handlers
                InputHandlerContainer.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, false);
            }
        }

        /// <summary>
        /// Stops rendering.
        /// </summary>
        private void StopRendering()
        {
            if (this.DesignMode) { return; }
            if (!FrozenSkyApplication.IsInitialized) { return; }

            if (m_renderLoop.IsRegisteredOnMainLoop)
            {
                m_renderLoop.DiscardRendering = true;
                m_renderLoop.DeregisterRenderLoop();

                // Updates currently active input handlers
                InputHandlerContainer.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, true);
            }
        }

        /// <summary>
        /// Updates the background brush used for failover rendering in System.Drawing.
        /// </summary>
        private void UpdateDrawingResourcesForFailoverRendering()
        {
            GraphicsHelper.SafeDispose(ref m_backBrush);
            GraphicsHelper.SafeDispose(ref m_foreBrushText);
            GraphicsHelper.SafeDispose(ref m_backBrushText);
            GraphicsHelper.SafeDispose(ref m_borderPen);

            m_backBrush = new System.Drawing.Drawing2D.HatchBrush(
                GDI.Drawing2D.HatchStyle.DottedGrid,
                GDI.Color.Gray, this.BackColor);
            m_backBrushText = new SolidBrush(GDI.Color.White);
            m_foreBrushText = new SolidBrush(GDI.Color.Black);
            m_borderPen = new Pen(GDI.Color.DarkGray);
        }

        /// <summary>
        /// Called when BackColor property has changed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            //Update background brush
            UpdateDrawingResourcesForFailoverRendering();
        }

        /// <summary>
        /// Called when the size of the viewport has changed.
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.DesignMode) { return; }
            if (!GraphicsCore.IsInitialized) { return; }

            if ((this.Width > 0) && (this.Height > 0))
            {
                m_renderLoop.Camera.SetScreenSize(this.Width, this.Height);
            }
        }

        /// <summary>
        /// Called when the window handle is created.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            StartRendering();
        }

        /// <summary>
        /// Called when the window handle is destroyed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            StopRendering();
        }

        /// <summary>
        /// Handle changed control visibility.
        /// </summary>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if(this.Visible){ StartRendering(); }
            else if(!this.Visible){ StopRendering(); }
        }

        /// <summary>
        /// Called when this view gets disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            if(!this.DesignMode)
            {
                // Updates currently active input handlers
                InputHandlerContainer.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, true);

                m_renderLoop.Dispose();
                m_renderLoop = null;
            }
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SharpDX.ViewportF, Size2, DpiScaling> OnRenderLoopCreateViewResources(EngineDevice device)
        {
            int width = this.Width;
            int height = this.Height;
            if (width <= RenderLoop.MIN_VIEW_WIDTH) { width = RenderLoop.MIN_VIEW_WIDTH; }
            if (height <= RenderLoop.MIN_VIEW_HEIGHT) { height = RenderLoop.MIN_VIEW_HEIGHT; }

            //Get all factories
            m_factory = device.FactoryDxgi;

            //Get all devices
            m_renderDevice = device.DeviceD3D11;
            m_renderDeviceContext = m_renderDevice.ImmediateContext;

            //Create the swap chain and the render target
            m_swapChain = GraphicsHelper.CreateDefaultSwapChain(this, device, m_renderLoop.ViewConfiguration);
            m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTarget = new D3D11.RenderTargetView(m_renderDevice, m_backBuffer);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(device, width, height, m_renderLoop.ViewConfiguration);
            m_renderTargetDepth = new D3D11.DepthStencilView(m_renderDevice, m_depthBuffer);

            //Define the viewport for rendering
            SharpDX.ViewportF viewPort = GraphicsHelper.CreateDefaultViewport(width, height);

            // Query for current dpi value
            DpiScaling dpiScaling = DpiScaling.Default;
            using (Graphics graphics = this.CreateGraphics())
            {
                dpiScaling.DpiX = graphics.DpiX;
                dpiScaling.DpiY = graphics.DpiY;
            }

            //Return all generated objects
            return Tuple.Create(m_backBuffer, m_renderTarget, m_depthBuffer, m_renderTargetDepth, viewPort, new Size2(width, height), dpiScaling);
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources(EngineDevice device)
        {
            m_factory = null;
            m_renderDevice = null;
            m_renderDeviceContext = null;

            m_renderTargetDepth = GraphicsHelper.DisposeObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeObject(m_depthBuffer);
            m_renderTarget = GraphicsHelper.DisposeObject(m_renderTarget);
            m_backBuffer = GraphicsHelper.DisposeObject(m_backBuffer);
            m_swapChain = GraphicsHelper.DisposeObject(m_swapChain);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender(EngineDevice device)
        {
            //if (!m_initialized) { return false; }
            if (this.Width <= 0) { return false; }
            if (this.Height <= 0) { return false; }
            //if (m_isOnRendering) { return false; }

            return true;
        }

        /// <summary>
        /// Called when the render loop prepares rendering.
        /// </summary>
        /// <param name="device">The current rendering device.</param>
        private void OnRenderLoopPrepareRendering(EngineDevice device)
        {
            if((m_renderLoop != null) &&
               (m_renderLoop.Camera != null))
            {
                this.UpdateMovement();
            }
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent(EngineDevice device)
        {
            //Present all rendered stuff on screen
            m_swapChain.Present(0, DXGI.PresentFlags.None);
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering(EngineDevice device)
        {
            //m_isOnRendering = false;
            if(!this.Visible)
            {
                StopRendering();
            }
        }

        /// <summary>
        /// Called when the currently active camera has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnRenderLoopCameraChanged(object sender, EventArgs e)
        {
            // Updates currently active input handlers
            InputHandlerContainer.UpdateInputHandlerList(this, m_inputHandlers, m_renderLoop, false);
        }

        /// <summary>
        /// Called when the control has changed its size.
        /// </summary>
        private void OnThrottledViewRecreation()
        {
            if (!this.DesignMode)
            {
                m_renderLoop.SetCurrentViewSize(this.Width, this.Height);

                //if ((this.Width > 0) && (this.Height > 0))
                //{
                //    m_renderLoop.RefreshViewResources();
                //}
            }
        }

        /// <summary>
        /// Called when the mouse is inside the area of this control.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            m_isMouseInside = true;
        }

        /// <summary>
        /// Called when the mouse is outside the area of this control.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            m_isMouseInside = false;
        }

        /// <summary>
        /// Called when RenderLoop allows it to manipulate current filter list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The decimal.</param>
        private void OnRenderLoopManipulateFilterList(object sender, ManipulateFilterListArgs e)
        {
            ManipulateFilterList.Raise(this, e);
        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        [Category("Rendering")]
        [DefaultValue(false)]
        public bool DiscardRendering
        {
            get { return m_renderLoop.DiscardRendering; }
            set { m_renderLoop.DiscardRendering = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            get { return m_renderLoop.Scene; }
            set { m_renderLoop.SetScene(value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera3DBase Camera
        {
            get { return m_renderLoop.Camera; }
            set { m_renderLoop.Camera = value; }
        }

        /// <summary>
        /// Gets the view configuration.
        /// </summary>
        [Browsable(true)]
        [Category("Rendering")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GraphicsViewConfiguration ViewConfiguration
        {
            get { return m_renderLoop.ViewConfiguration; }
        }

        /// <summary>
        /// Gets the render loop object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        /// <summary>
        /// Ruft die Hintergrundfarbe für das Steuerelement ab oder legt diese fest.
        /// </summary>
        /// <returns>Eine <see cref="T:System.Drawing.Color" />, die die Hintergrundfarbe des Steuerelements darstellt. Der Standardwert ist der Wert der <see cref="P:System.Windows.Forms.Control.DefaultBackColor" />-Eigenschaft.</returns>
        public override GDI.Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                m_renderLoop.ClearColor = new Color4(this.BackColor);
            }
        }

        [Browsable(false)]
        public EngineDevice Device
        {
            get
            {
                if (m_renderLoop != null) { return m_renderLoop.Device; }
                else { return null; }
            }
        }
    }
}