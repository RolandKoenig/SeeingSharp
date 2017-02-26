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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.Graphics.Holographic;
using Windows.Perception.Spatial;
using Windows.Foundation;
using SeeingSharp.Multimedia.Input;
using SeeingSharp.Multimedia.Core;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using MS_D3D11 = Windows.Graphics.DirectX.Direct3D11;
using SDM = SharpDX.Mathematics.Interop;

namespace SeeingSharp.Multimedia.Views
{
    public class SeeingSharpHolographicsSpacePainter : IDisposable, IInputEnabledView, IRenderLoopHost
    {
        private CoreWindow m_targetWindow;

        #region Holographic data
        private HolographicSpace m_holoSpace;
        private SpatialStationaryFrameOfReference m_referenceFrame;
        private SpatialLocator m_spatialLocator;
        #endregion

        #region Adapter references
        private EngineDevice m_hostDevice;
        private DXGI.Adapter3 m_dxgiAdapter;
        private MS_D3D11.IDirect3DDevice m_d3dInteropDevice;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpHolographicsSpacePainter"/> class.
        /// </summary>
        /// <param name="targetWindow">The target window into which to render.</param>
        public SeeingSharpHolographicsSpacePainter(CoreWindow targetWindow)
        {
            m_targetWindow = targetWindow;

            // Call generic initialization
            //  (choose device, create holographic space)
            InitializeHolographicSpace(targetWindow);

            // Use the default SpatialLocator to track the motion of the device.
            m_spatialLocator = SpatialLocator.GetDefault();

            // Be able to respond to changes in the positional tracking state.
            m_spatialLocator.LocatabilityChanged += this.OnSpatialLocator_LocatabilityChanged;

            // Respond to camera added events by creating any resources that are specific
            // to that camera, such as the back buffer render target view.
            // When we add an event handler for CameraAdded, the API layer will avoid putting
            // the new camera in new HolographicFrames until we complete the deferral we created
            // for that handler, or return from the handler without creating a deferral. This
            // allows the app to take more than one frame to finish creating resources and
            // loading assets for the new holographic camera.
            // This function should be registered before the app creates any HolographicFrames.
            m_holoSpace.CameraAdded += this.OnHoloSpace_CameraAdded;

            // Respond to camera removed events by releasing resources that were created for that
            // camera.
            // When the app receives a CameraRemoved event, it releases all references to the back
            // buffer right away. This includes render target views, Direct2D target bitmaps, and so on.
            // The app must also ensure that the back buffer is not attached as a render target, as
            // shown in DeviceResources.ReleaseResourcesForBackBuffer.
            m_holoSpace.CameraRemoved += this.OnHoloSpace_CameraRemoved;

            // The simplest way to render world-locked holograms is to create a stationary reference frame
            // when the app is launched. This is roughly analogous to creating a "world" coordinate system
            // with the origin placed at the device's position as the app is launched.
            m_referenceFrame = m_spatialLocator.CreateStationaryFrameOfReferenceAtCurrentLocation();
        }

        public void Dispose()
        {
            GraphicsHelper.SafeDispose(ref m_d3dInteropDevice);
            GraphicsHelper.SafeDispose(ref m_dxgiAdapter);
        }

        public Tuple<D3D11.Texture2D, D3D11.RenderTargetView, D3D11.Texture2D, D3D11.DepthStencilView, SDM.RawViewportF, Size2, DpiScaling> OnRenderLoop_CreateViewResources(EngineDevice device)
        {
            return null;
        }

        private void InitializeHolographicSpace(CoreWindow targetWindow)
        {
            // Create the holographic space
            m_holoSpace = HolographicSpace.CreateForCoreWindow(targetWindow);

            // The holographic space might need to determine which adapter supports
            // holograms, in which case it will specify a non-zero PrimaryAdapterId.
            int shiftPos = sizeof(uint);
            ulong id = (ulong)m_holoSpace.PrimaryAdapterId.LowPart | (((ulong)m_holoSpace.PrimaryAdapterId.HighPart) << shiftPos);

            // Get the device with the luid or get the default device of the current system
            if (id != 0)
            {
                m_hostDevice = GraphicsCore.Current.TryGetDeviceByLuid((long)id);
            }
            if (m_hostDevice == null) { m_hostDevice = GraphicsCore.Current.DefaultDevice; }

            // Acquire the DXGI interface for the Direct3D device.
            D3D11.Device3 d3dDevice = m_hostDevice.Device3D11_3;
            using (var dxgiDevice = d3dDevice.QueryInterface<SharpDX.DXGI.Device3>())
            {
                // Wrap the native device using a WinRT interop object.
                IntPtr pUnknown;
                UInt32 hr = NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out pUnknown);
                if (hr == 0)
                {
                    m_d3dInteropDevice = (MS_D3D11.IDirect3DDevice)Marshal.GetObjectForIUnknown(pUnknown);
                    Marshal.Release(pUnknown);
                }

                // Store a pointer to the DXGI adapter.
                // This is for the case of no preferred DXGI adapter, or fallback to WARP.
                m_dxgiAdapter = dxgiDevice.Adapter.QueryInterface<SharpDX.DXGI.Adapter3>();
            }

            // Set the Direct3D device on the holographic space
            m_holoSpace.SetDirect3D11Device(m_d3dInteropDevice);
        }

        void IRenderLoopHost.OnRenderLoop_DisposeViewResources(EngineDevice device)
        {

        }

        bool IRenderLoopHost.OnRenderLoop_CheckCanRender(EngineDevice device)
        {
            return true;
        }

        void IRenderLoopHost.OnRenderLoop_PrepareRendering(EngineDevice device)
        {
            // Before doing the timer update, there is some work to do per-frame
            // to maintain holographic rendering. First, we will get information
            // about the current frame.

            // The HolographicFrame has information that the app needs in order
            // to update and render the current frame. The app begins each new
            // frame by calling CreateNextFrame.
            HolographicFrame holographicFrame = m_holoSpace.CreateNextFrame();

            // Get a prediction of where holographic cameras will be when this frame
            // is presented.
            HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;

            // Next, we get a coordinate system from the attached frame of reference that is
            // associated with the current frame. Later, this coordinate system is used for
            // for creating the stereo view matrices when rendering the sample content.
            SpatialCoordinateSystem currentCoordinateSystem = m_referenceFrame.CoordinateSystem;
        }

        void IRenderLoopHost.OnRenderLoop_AfterRendering(EngineDevice device)
        {

        }

        void IRenderLoopHost.OnRenderLoop_Present(EngineDevice device)
        {

        }

        public void OnHoloSpace_CameraAdded(
    HolographicSpace sender,
    HolographicSpaceCameraAddedEventArgs args)
        {
            Deferral deferral = args.GetDeferral();
            HolographicCamera holographicCamera = args.Camera;

            Task task1 = new Task(() =>
            {
                //
                // TODO: Allocate resources for the new camera and load any content specific to
                //       that camera. Note that the render target size (in pixels) is a property
                //       of the HolographicCamera object, and can be used to create off-screen
                //       render targets that match the resolution of the HolographicCamera.
                //

                // Create device-based resources for the holographic camera and add it to the list of
                // cameras used for updates and rendering. Notes:
                //   * Since this function may be called at any time, the AddHolographicCamera function
                //     waits until it can get a lock on the set of holographic camera resources before
                //     adding the new camera. At 60 frames per second this wait should not take long.
                //   * A subsequent Update will take the back buffer from the RenderingParameters of this
                //     camera's CameraPose and use it to create the ID3D11RenderTargetView for this camera.
                //     Content can then be rendered for the HolographicCamera.
                //deviceResources.AddHolographicCamera(holographicCamera);

                // Holographic frame predictions will not include any information about this camera until
                // the deferral is completed.
                deferral.Complete();
            });
            task1.Start();
        }

        public void OnHoloSpace_CameraRemoved(
            HolographicSpace sender,
            HolographicSpaceCameraRemovedEventArgs args)
        {
            Task task2 = new Task(() =>
            {
                //
                // TODO: Asynchronously unload or deactivate content resources (not back buffer 
                //       resources) that are specific only to the camera that was removed.
                //
            });
            task2.Start();

            // Before letting this callback return, ensure that all references to the back buffer 
            // are released.
            // Since this function may be called at any time, the RemoveHolographicCamera function
            // waits until it can get a lock on the set of holographic camera resources before
            // deallocating resources for this camera. At 60 frames per second this wait should
            // not take long.
            //deviceResources.RemoveHolographicCamera(args.Camera);
        }

        private void OnSpatialLocator_LocatabilityChanged(SpatialLocator sender, Object args)
        {
            switch (sender.Locatability)
            {
                case SpatialLocatability.Unavailable:
                    // Holograms cannot be rendered.
                    break;

                // In the following three cases, it is still possible to place holograms using a
                // SpatialLocatorAttachedFrameOfReference.
                case SpatialLocatability.PositionalTrackingActivating:
                // The system is preparing to use positional tracking.
                case SpatialLocatability.OrientationOnly:
                // Positional tracking has not been activated.
                case SpatialLocatability.PositionalTrackingInhibited:
                    // Positional tracking is temporarily inhibited. User action may be required
                    // in order to restore positional tracking.
                    break;

                case SpatialLocatability.PositionalTrackingActive:
                    // Positional tracking is active. World-locked content can be rendered.
                    break;
            }
        }

        public bool Focused
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public RenderLoop RenderLoop
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
