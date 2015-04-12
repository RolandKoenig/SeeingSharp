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

using System.ComponentModel;

using DXGI = SharpDX.DXGI;

namespace FrozenSky.Multimedia.Core
{
    public class EngineOutputInfo 
    {
        private const string TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO = "Common output information";

        private int m_outputIndex;
        private DXGI.OutputDescription m_outputDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputItemViewModel" /> class.
        /// </summary>
        /// <param name="output">The output to get all data from.</param>
        internal EngineOutputInfo(int outputIndex, DXGI.Output output)
        {
            m_outputIndex = outputIndex;
            m_outputDescription = output.Description;
        }

        /// <summary>
        /// Gets the name of the output device.
        /// </summary>
        [Category(TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO)]
        public string DeviceName
        {
            get { return m_outputDescription.DeviceName; }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO)]
        public bool IsAttachedToDesktop
        {
            get { return m_outputDescription.IsAttachedToDesktop; }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO)]
        public string DesktopResolution
        {
            get { return m_outputDescription.DesktopBounds.Width + "x" + m_outputDescription.DesktopBounds.Height.ToString(); }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO)]
        public string DesktopLocation
        {
            get { return "X = " + m_outputDescription.DesktopBounds.X + ", Y = " + m_outputDescription.DesktopBounds.Y; }
        }

        [Category(TRANSLATABLE_GROUP_COMMON_OUTPUT_INFO)]
        public string Rotation
        {
            get { return m_outputDescription.Rotation.ToString(); }
        }
    }
}
