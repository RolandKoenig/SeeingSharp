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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Foundation;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

//Some namespace mappings
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Views
{
    public class SeeingSharpCoreWindowPainter : IDisposable, IInputEnabledView, IRenderLoopHost
    {
        #region Constants
        //private double MIN_PIXEL_SIZE_WIDTH = 16.0;
        //private double MIN_PIXEL_SIZE_HEIGHT = 16.0;
        #endregion

        #region Configuration
        private CoreWindow m_targetWindow;
        #endregion

        #region Main engine objects
        // private RenderLoop m_renderLoop;
        #endregion

        #region Resources from Direct3D 11
        //private DXGI.SwapChain1 m_swapChain;
        //private D3D11.Texture2D m_backBuffer;
        //private D3D11.Texture2D m_backBufferMultisampled;
        //private D3D11.Texture2D m_depthBuffer;
        //private D3D11.RenderTargetView m_renderTargetView;
        //private D3D11.DepthStencilView m_renderTargetDepth;
        #endregion

        public SeeingSharpCoreWindowPainter(CoreWindow targetWindow)
        {
            m_targetWindow = targetWindow;
            m_targetWindow.SizeChanged += OnTargetWindow_SizeChanged;
        }


        private void OnTargetWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {

        }

        public void Dispose()
        {

        }

        ///// <summary>
        ///// Gets the current target pixel size for the render panel.
        ///// </summary>
        //private Size2 GetTargetRenderPixelSize()
        //{
        //    double currentWidth = m_targetWindow.Bounds.Width;
        //    double currentHeight = m_targetWindow.Bounds.Height;

        //    return new Size2(
        //        (int)(currentWidth > MIN_PIXEL_SIZE_WIDTH ? currentWidth : MIN_PIXEL_SIZE_WIDTH),
        //        (int)(currentHeight > MIN_PIXEL_SIZE_HEIGHT ? currentHeight : MIN_PIXEL_SIZE_HEIGHT));
        //}

        public Tuple<Texture2D, RenderTargetView, Texture2D, DepthStencilView, RawViewportF, Size2, DpiScaling> OnRenderLoop_CreateViewResources(EngineDevice device)
        {
            return null;

            //m_backBufferMultisampled = null;

            //Size2 viewSize = GetTargetRenderPixelSize();

            //// Create the SwapChain and associate it with the SwapChainBackgroundPanel 
            //m_swapChain = GraphicsHelper.CreateSwapChainForComposition(device, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);

            //// Get the backbuffer from the SwapChain
            //m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);

            //// Define the render target (in case of multisample an own render target)
            //D3D11.Texture2D backBufferForRenderloop = null;
            //if (m_renderLoop.ViewConfiguration.AntialiasingEnabled)
            //{
            //    m_backBufferMultisampled = GraphicsHelper.CreateRenderTargetTexture(device, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            //    m_renderTargetView = new D3D11.RenderTargetView(device.DeviceD3D11, m_backBufferMultisampled);
            //    backBufferForRenderloop = m_backBufferMultisampled;
            //}
            //else
            //{
            //    m_renderTargetView = new D3D11.RenderTargetView(device.DeviceD3D11, m_backBuffer);
            //    backBufferForRenderloop = m_backBuffer;
            //}

            ////Create the depth buffer
            //m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(device, viewSize.Width, viewSize.Height, m_renderLoop.ViewConfiguration);
            //m_renderTargetDepth = new D3D11.DepthStencilView(device.DeviceD3D11, m_depthBuffer);

            ////Define the viewport for rendering
            //SharpDX.Mathematics.Interop.RawViewportF viewPort = GraphicsHelper.CreateDefaultViewport(viewSize.Width, viewSize.Height);
            ////m_lastRefreshTargetSize = new Size(viewSize.Width, viewSize.Height);

            //DpiScaling dpiScaling = new DpiScaling();
            //dpiScaling.DpiX = 96.0f;
            //dpiScaling.DpiY = 96.0f;

            //return Tuple.Create(backBufferForRenderloop, m_renderTargetView, m_depthBuffer, m_renderTargetDepth, viewPort, viewSize, dpiScaling);
        }

        public void OnRenderLoop_DisposeViewResources(EngineDevice device)
        {

        }

        public bool OnRenderLoop_CheckCanRender(EngineDevice device)
        {

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
            //// Present all rendered stuff on screen
            //// First parameter indicates synchronization with vertical blank
            ////  see http://msdn.microsoft.com/en-us/library/windows/desktop/bb174576(v=vs.85).aspx
            ////  see example http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx
            //m_swapChain.Present(1, DXGI.PresentFlags.None);
        }

        public bool Focused
        {
            get { return false; }
        }

        public RenderLoop RenderLoop
        {
            get { return null; } //return m_renderLoop; }
        }
    }
}
