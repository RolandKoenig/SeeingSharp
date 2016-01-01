#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
using SeeingSharp.Util;
using System;
using System.Xml.Serialization;

namespace SeeingSharp.Multimedia.Core
{
    public class GraphicsViewConfiguration 
    {
        private const string CATEOGRY_COMMON = "Common";
        private const string CATEGORY_QUALITY = "Quality";
        private const string CATEGORY_DETAILS = "Details";

        #region Generic
        private GraphicsDeviceConfiguration m_deviceConfig;
        private bool m_viewNeedsRefresh;
        #endregion

        #region Antialiasing configuration
        private bool m_antialiasingEnabled;
        private AntialiasingQualityLevel m_antialiasingQuality;
        #endregion

        #region Most view parameters (Light, Gradient, Accentuation)
        private float m_generatedColorGradientFactor;
        private float m_generatedBorderFactor;
        private float m_accentuationFactor;
        private float m_ambientFactor;
        private float m_lightPower;
        private float m_strongLightFactor;
        private bool m_alphaEnabledSwapChain;
        #endregion

        /// <summary>
        /// Occurs when any configuration flag has changed.
        /// This event may occure in different threads!
        /// </summary>
        public event EventHandler ConfigurationChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsViewConfiguration" /> class.
        /// </summary>
        internal GraphicsViewConfiguration()
        {
            this.ShowTextures = true;
            this.AntialiasingEnabled = true;
            this.AntialiasingQuality = AntialiasingQualityLevel.Medium;

            // Define and execute reset action
            Action resetAction = () =>
            {
                ShowTexturesInternal = true;
                m_generatedBorderFactor = 1f;
                m_generatedColorGradientFactor = 1f;
                m_accentuationFactor = 0f;
                m_ambientFactor = 0.2f;
                m_lightPower = 0.8f;
                m_strongLightFactor = 1.5f;
                m_alphaEnabledSwapChain = false;
            };
            resetAction();

            // Define commands
            ResetCommand = new DelegateCommand(resetAction);
        }

        public DelegateCommand ResetCommand
        {
            get;
            private set;
        }

        internal bool ViewNeedsRefresh
        {
            get { return m_viewNeedsRefresh; }
            set { m_viewNeedsRefresh = value; }
        }

        /// <summary>
        /// Is wireframe rendering enabled?
        /// </summary>
        public bool WireframeEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Try to enable antialiasing?
        /// </summary>
        public bool AntialiasingEnabled
        {
            get { return m_antialiasingEnabled; }
            set
            {
                if (m_antialiasingEnabled != value)
                {
                    m_antialiasingEnabled = value;
                    m_viewNeedsRefresh = true;

                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The quality level for antialiasing (if antialiasing is enabled).
        /// </summary>
        public AntialiasingQualityLevel AntialiasingQuality
        {
            get { return m_antialiasingQuality; }
            set
            {
                if (m_antialiasingQuality != value)
                {
                    m_antialiasingQuality = value;
                    m_viewNeedsRefresh = true;

                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float GeneratedColorGradientFactor
        {
            get { return m_generatedColorGradientFactor; }
            set
            {
                if (m_generatedColorGradientFactor != value)
                {
                    m_generatedColorGradientFactor = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float GeneratedBorderFactor
        {
            get { return m_generatedBorderFactor; }
            set
            {
                if (m_generatedBorderFactor != value)
                {
                    m_generatedBorderFactor = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float AccentuationFactor
        {
            get { return m_accentuationFactor; }
            set
            {
                if (m_accentuationFactor != value)
                {
                    m_accentuationFactor = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float AmbientFactor
        {
            get { return m_ambientFactor; }
            set
            {
                if (m_ambientFactor != value)
                {
                    m_ambientFactor = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float LightPower
        {
            get { return m_lightPower; }
            set
            {
                if (m_lightPower != value)
                {
                    m_lightPower = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float StrongLightFactor
        {
            get { return m_strongLightFactor; }
            set
            {
                if (m_strongLightFactor != value)
                {
                    m_strongLightFactor = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Internal accessor for ShowTextures variable.
        /// </summary>
        internal bool ShowTexturesInternal;

        [XmlAttribute]
        public bool ShowTextures
        {
            get { return ShowTexturesInternal; }
            set
            {
                if (ShowTexturesInternal != value)
                {
                    ShowTexturesInternal = value;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Set this flag to true to enable transparent pixels when view is embedded into Xaml stack.
        /// Only relevant in UWP-Apps!
        /// </summary>
        [XmlAttribute]
        public bool AlphaEnabledSwapChain
        {
            get { return m_alphaEnabledSwapChain; }
            set
            {
                if(m_alphaEnabledSwapChain != value)
                {
                    m_alphaEnabledSwapChain = value;
                    m_viewNeedsRefresh = true;
                    ConfigurationChanged.Raise(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets current device configuration.
        /// </summary>
        public GraphicsDeviceConfiguration DeviceConfiguration
        {
            get { return m_deviceConfig; }
            internal set { m_deviceConfig = value; }
        }

        /// <summary>
        /// Gets current core configuration.
        /// </summary>
        public GraphicsCoreConfiguration CoreConfiguration
        {
            get 
            {
                if (m_deviceConfig == null) { return null; }
                return m_deviceConfig.CoreConfiguration;
            }
        }
    }
}
