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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
#if UNIVERSAL
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using D2D = SharpDX.Direct2D1;
#endif
#if DESKTOP
using DXGI = SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using D3D9 = SharpDX.Direct3D9;
using DWrite = SharpDX.DirectWrite;
using D2D = SharpDX.Direct2D1;
#endif

namespace SeeingSharp.Multimedia.Core
{
    public class EngineDevice
    {
        #region Constants
        private const string CATEGORY_ADAPTER = "Adapter";
        #endregion

        #region Main members
        private DXGI.Adapter1 m_adapter;
        private GraphicsDeviceConfiguration m_configuration;
        private GraphicsCore m_core;
        private bool m_isSoftwareAdapter;
        private bool m_debugEnabled;
        private Exception m_initializationException;
        #endregion

        #region Some configuration
        private bool m_isDetailLevelForced;
        private DetailLevel m_forcedDetailLevel;
        #endregion

        #region Handlers for different DirectX Apis
#if UNIVERSAL
        private DeviceHandlerDXGI m_handlerDXGI;
        private DeviceHandlerD3D11 m_handlerD3D11;
        private DeviceHandlerD2D m_handlerD2D;
#endif
#if DESKTOP
        private DeviceHandlerDXGI m_handlerDXGI;
        private DeviceHandlerD3D11 m_handlerD3D11;
        private DeviceHandlerD3D9 m_handlerD3D9;
        private DeviceHandlerD2D m_handlerD2D;
#endif
        #endregion

        #region Possible antialiasing modes
        private DXGI.SampleDescription m_antialiasingConfigLow;
        private DXGI.SampleDescription m_antialiasingConfigMedium;
        private DXGI.SampleDescription m_antialiasingConfigHigh;
        #endregion

        #region Members for antialiasing
        private bool m_isStandardAntialiasingSupported;
        private DXGI.SampleDescription m_sampleDescWithAntialiasing;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineDevice"/> class.
        /// </summary>
        internal EngineDevice(GraphicsCore core, GraphicsCoreConfiguration coreConfiguration, DXGI.Adapter1 adapter, bool isSoftwareAdapter, bool debugEnabled)
        {
            m_core = core;
            m_adapter = adapter;
            m_isSoftwareAdapter = isSoftwareAdapter;
            m_debugEnabled = debugEnabled;
            m_configuration = new GraphicsDeviceConfiguration(coreConfiguration);

            // Set default antialiasing configurations
            m_sampleDescWithAntialiasing = new DXGI.SampleDescription(1, 0);

            // Initialize all direct3D APIs
            try
            {
#if UNIVERSAL
                m_handlerD3D11 = new DeviceHandlerD3D11(adapter, debugEnabled);
                m_handlerDXGI = new DeviceHandlerDXGI(adapter, m_handlerD3D11.Device);
#endif
#if DESKTOP
                m_handlerD3D11 = new DeviceHandlerD3D11(adapter, debugEnabled);
                m_handlerDXGI = new DeviceHandlerDXGI(adapter, m_handlerD3D11.Device);
                m_handlerD3D9 = new DeviceHandlerD3D9(adapter, isSoftwareAdapter, debugEnabled); 
#endif
            }
            catch (Exception ex)
            {
                m_initializationException = ex;

#if UNIVERSAL
                m_handlerD3D11 = null;
                m_handlerDXGI = null;
#endif
#if DESKTOP
                m_handlerD3D11 = null;
                m_handlerDXGI = null;
                m_handlerD3D9 = null;
#endif
            }

            // Set default configuration
            m_configuration.TextureQuality = !isSoftwareAdapter && m_handlerD3D11.IsDirect3D10OrUpperHardware ? TextureQuality.Hight : TextureQuality.Low;
            m_configuration.GeometryQuality = !isSoftwareAdapter && m_handlerD3D11.IsDirect3D10OrUpperHardware ? GeometryQuality.Hight : GeometryQuality.Low;

            // Initialize handlers for feature support information
            if (m_initializationException == null)
            {
                m_isStandardAntialiasingSupported = CheckIsStandardAntialiasingPossible();
            }

            // Initialize direct2D handler finally
            if (m_handlerD3D11 != null)
            {
                m_handlerD2D = new DeviceHandlerD2D(m_core, this);
                this.FakeRenderTarget2D = m_handlerD2D.RenderTarget;
            }
        }

        /// <summary>
        /// Forces the given detail level.
        /// </summary>
        public void ForceDetailLevel(DetailLevel detailLevel)
        {
            m_isDetailLevelForced = true;
            m_forcedDetailLevel = detailLevel;
        }

        /// <summary>
        /// Get the sample description for the given quality level.
        /// </summary>
        /// <param name="qualityLevel">The quality level for which a sample description is needed.</param>
        internal DXGI.SampleDescription GetSampleDescription(AntialiasingQualityLevel qualityLevel)
        {
            switch (qualityLevel)
            {
                case AntialiasingQualityLevel.Low:
                    return m_antialiasingConfigLow;

                case AntialiasingQualityLevel.Medium:
                    return m_antialiasingConfigMedium;

                case AntialiasingQualityLevel.High:
                    return m_antialiasingConfigHigh;
            }

            return new DXGI.SampleDescription(1, 0);
        }

        /// <summary>
        /// Creates a list containing all available devices.
        /// </summary>
        public static IEnumerable<EngineDevice> CreateDevices(GraphicsCore core, GraphicsCoreConfiguration coreConfiguration, bool debugEnabled)
        {
            using(DXGI.Factory1 dxgiFactory = new DXGI.Factory1())
            {
                int adapterCount = dxgiFactory.GetAdapterCount1();
                for(int loop=0 ; loop<adapterCount; loop++)
                {
                    DXGI.Adapter1 actAdapter = dxgiFactory.GetAdapter1(loop);
                    bool isSoftware = actAdapter.Description1.Description.StartsWith("Microsoft Basic Render Driver");

                    yield return new EngineDevice(core, coreConfiguration, actAdapter, isSoftware, debugEnabled);
                }
            }
        }

        /// <summary>
        /// Get the sample description for the given quality level.
        /// </summary>
        internal DXGI.SampleDescription GetSampleDescription(bool antialiasingEnabled)
        {
            if (antialiasingEnabled) { return m_sampleDescWithAntialiasing; }
            else { return new DXGI.SampleDescription(1, 0); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (m_initializationException != null) { return m_adapter.Description1.Description; }
            else { return m_handlerD3D11.DeviceModeDescription; }
        }

        /// <summary>
        /// Checks for standard antialiasing support.
        /// </summary>
        private bool CheckIsStandardAntialiasingPossible()
        {
            // Very important to check possible antialiasing
            // More on the used technique
            //  see http://msdn.microsoft.com/en-us/library/windows/apps/dn458384.aspx

            D3D11.FormatSupport formatSupport = m_handlerD3D11.Device.CheckFormatSupport(GraphicsHelper.DEFAULT_TEXTURE_FORMAT);
            if ((formatSupport & D3D11.FormatSupport.MultisampleRenderTarget) != D3D11.FormatSupport.MultisampleRenderTarget) { return false; }
            if ((formatSupport & D3D11.FormatSupport.MultisampleResolve) != D3D11.FormatSupport.MultisampleResolve) { return false; }
            if (m_handlerD3D11.FeatureLevel == SharpDX.Direct3D.FeatureLevel.Level_9_1) { return false; }

            try
            {
                D3D11.Texture2DDescription textureDescription = new D3D11.Texture2DDescription();
                textureDescription.Width = 100;
                textureDescription.Height = 100;
                textureDescription.MipLevels = 1;
                textureDescription.ArraySize = 1;
                textureDescription.Format = GraphicsHelper.DEFAULT_TEXTURE_FORMAT;
                textureDescription.Usage = D3D11.ResourceUsage.Default;
                textureDescription.SampleDescription = new DXGI.SampleDescription(2, 0);
                textureDescription.BindFlags = D3D11.BindFlags.ShaderResource | D3D11.BindFlags.RenderTarget;
                textureDescription.CpuAccessFlags = D3D11.CpuAccessFlags.None;
                textureDescription.OptionFlags = D3D11.ResourceOptionFlags.None;
                D3D11.Texture2D testTexture = new D3D11.Texture2D(m_handlerD3D11.Device, textureDescription);
                GraphicsHelper.SafeDispose(ref testTexture);
            }
            catch(Exception)
            {
                return false;
            }

            // Check for quality levels
            int lowQualityLevels = m_handlerD3D11.Device.CheckMultisampleQualityLevels(GraphicsHelper.DEFAULT_TEXTURE_FORMAT, 2);
            int mediumQualityLevels = m_handlerD3D11.Device.CheckMultisampleQualityLevels(GraphicsHelper.DEFAULT_TEXTURE_FORMAT, 4);
            int hightQualityLevels = m_handlerD3D11.Device.CheckMultisampleQualityLevels(GraphicsHelper.DEFAULT_TEXTURE_FORMAT, 8);

            // Generate sample descriptions for each possible quality level
            if (lowQualityLevels > 0)
            {
                m_antialiasingConfigLow = new DXGI.SampleDescription(2, lowQualityLevels - 1);
            }
            if (mediumQualityLevels > 0)
            {
                m_antialiasingConfigMedium = new DXGI.SampleDescription(4, mediumQualityLevels - 1);
            }
            if (hightQualityLevels > 0)
            {
                m_antialiasingConfigHigh = new DXGI.SampleDescription(8, hightQualityLevels - 1);
            }

            return lowQualityLevels > 0;
        }

        /// <summary>
        /// Checks for standard antialiasing support.
        /// </summary>
        public bool IsStandardAntialiasingPossible
        {
            get 
            {
                return m_isStandardAntialiasingSupported; 
            }
        }

        /// <summary>
        /// Gets the exception occurred during initialization of the driver (if any).
        /// </summary>
        public Exception InitializationException
        {
            get { return m_initializationException; }
        }

        /// <summary>
        /// Gets the description of this adapter.
        /// </summary>
        public string AdapterDescription
        {
            get { return m_adapter.Description1.Description.Replace("\0", ""); }
        }

        /// <summary>
        /// Is this device loaded successfully.
        /// </summary>
        public bool IsLoadedSuccessfully
        {
            get { return m_initializationException == null; }
        }

        public bool IsSoftware
        {
            get { return m_isSoftwareAdapter; }
        }

        /// <summary>
        /// Gets the supported detail level of this device.
        /// </summary>
        public DetailLevel SupportedDetailLevel
        {
            get
            {
                if (m_isDetailLevelForced) { return m_forcedDetailLevel; }

                if (m_isSoftwareAdapter) { return DetailLevel.Low; }
                else { return DetailLevel.High; }
            }
        }

        /// <summary>
        /// Is high detail supported with this card?
        /// </summary>
        public bool IsHighDetailSupported
        {
            get
            {
                return (this.SupportedDetailLevel | DetailLevel.High) == DetailLevel.High;
            }
        }

        /// <summary>
        /// Gets the level of the graphics driver.
        /// </summary>
        public HardwareDriverLevel DriverLevel
        {
            get
            {
                if (m_handlerD3D11 != null) { return m_handlerD3D11.DriverLevel; }
                return HardwareDriverLevel.Direct3D11;
            }
        }

        /// <summary>
        /// Gets the name of the default shader model.
        /// </summary>
        public string DefaultPixelShaderModel
        {
            get
            {
                switch(DriverLevel)
                {
                    case HardwareDriverLevel.Direct3D9_1:
                    case HardwareDriverLevel.Direct3D9_2:
                        return "ps_4_0_level_9_1";

                    case HardwareDriverLevel.Direct3D9_3:
                        return "ps_4_0_level_9_3";

                    case HardwareDriverLevel.Direct3D11:
                        return "ps_5_0";

                    default:
                        return "ps_4_0";
                }
            }
        }

        /// <summary>
        /// Some older hardware only support 16-bit index buffers.
        /// </summary>
        public bool SupportsOnly16BitIndexBuffer
        {
            get
            {
                // see https://msdn.microsoft.com/en-us/library/windows/desktop/ff471324(v=vs.85).aspx
                return DriverLevel == HardwareDriverLevel.Direct3D9_1;
            }
        }

        /// <summary>
        /// Gets the name of the default shader model.
        /// </summary>
        public string DefaultVertexShaderModel
        {
            get
            {
                switch (DriverLevel)
                {
                    case HardwareDriverLevel.Direct3D9_1:
                    case HardwareDriverLevel.Direct3D9_2:
                        return "vs_4_0_level_9_1";

                    case HardwareDriverLevel.Direct3D9_3:
                        return "vs_4_0_level_9_3";

                    case HardwareDriverLevel.Direct3D11:
                        return "vs_5_0";

                    default:
                        return "vs_4_0";
                }
            }
        }

        public GraphicsCore Core
        {
            get { return m_core; }
        }

        /// <summary>
        /// Gets the Direct3D 11 device object.
        /// </summary>
        internal D3D11.Device1 DeviceD3D11
        {
            get { return m_handlerD3D11.Device; }
        }

#if DESKTOP
        internal D3D9.DeviceEx DeviceD3D9
        {
            get { return m_handlerD3D9.Device; }
        }

        internal D3D9.Direct3DEx ContextD3D9
        {
            get { return m_handlerD3D9.Context; }
        }

        internal D2D.Device DeviceD2D
        {
            get { return m_handlerD2D.Device; }
        }

        internal D2D.DeviceContext DeviceContextD2D
        {
            get { return m_handlerD2D.DeviceContext; }
        }
#else
        internal D2D.Device DeviceD2D
        {
            get { return m_handlerD2D.Device; }
        }

        internal D2D.DeviceContext DeviceContextD2D
        {
            get { return m_handlerD2D.DeviceContext; }
        }

        internal DXGI.Device3 DeviceDxgi
        {
            get { return m_handlerDXGI.Device; }
        }
#endif

        public bool IsUsingFallbackMethodFor2D
        {
            get
            {
                return
                    (m_core.Force2DFallbackMethod) ||
                    (m_core.FactoryD2D_2 == null) ||
                    ((m_handlerD2D != null) && (m_handlerD2D.IsUsingFallbackMethod));
            }
        }

        /// <summary>
        /// Gets the main Direct3D 11 context object.
        /// </summary>
        internal D3D11.DeviceContext DeviceImmediateContextD3D11
        {
            get { return m_handlerD3D11.ImmediateContext; }
        }

        /// <summary>
        /// Gets the current device configuration.
        /// </summary>
        public GraphicsDeviceConfiguration Configuration
        {
            get { return m_configuration; }
        }

#if DESKTOP
        /// <summary>
        /// Gets the DXGI factory object.
        /// </summary>
        internal DXGI.Factory2 FactoryDxgi
        {
            get { return m_handlerDXGI.Factory; }
        }
#else
        /// <summary>
        /// Gets the DXGI factory object.
        /// </summary>
        internal DXGI.Factory2 FactoryDxgi
        {
            get
            {
                return m_handlerDXGI.Factory;
            }
        }
#endif

        /// <summary>
        /// Gets the 2D render target which can be used to load 2D resources on this device.
        /// </summary>
        internal D2D.RenderTarget FakeRenderTarget2D;

        /// <summary>
        /// Gets the index of this device.
        /// </summary>
        internal int DeviceIndex;
    }
}
