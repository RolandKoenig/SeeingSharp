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
using System.ComponentModel;

using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Core
{
    public class EngineOutputInfo 
    {
        private const string TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO = "Common output information";

        private int m_outputIndex;
        private DXGI.OutputDescription m_outputDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineOutputInfo" /> class.
        /// </summary>
        internal EngineOutputInfo(int outputIndex, DXGI.Output output)
        {
            m_outputIndex = outputIndex;
            m_outputDescription = output.Description;
        }

        /// <summary>
        /// Gets the name of the output device.
        /// </summary>
        public string DeviceName
        {
            get { return m_outputDescription.DeviceName; }
        }

        public bool IsAttachedToDesktop
        {
            get { return m_outputDescription.IsAttachedToDesktop; }
        }

        public string DesktopResolution
        {
            get
            {
                return 
                    (m_outputDescription.DesktopBounds.Right - m_outputDescription.DesktopBounds.Left) + 
                    "x" +
                    (m_outputDescription.DesktopBounds.Bottom - m_outputDescription.DesktopBounds.Top);
            }
        }

        public string DesktopLocation
        {
            get
            {
                return "X = " + m_outputDescription.DesktopBounds.Left + ", Y = " + m_outputDescription.DesktopBounds.Top;
            }
        }

        public string Rotation
        {
            get { return m_outputDescription.Rotation.ToString(); }
        }
    }
}
