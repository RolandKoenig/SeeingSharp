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

#if !WINDOWS_PHONE
//Some namespace mappings
using DWrite = SharpDX.DirectWrite;

namespace FrozenSky.Multimedia.Core
{
    public class FactoryHandlerDWrite
    {
        private GraphicsCore m_core;

        //Resources for DirectWrite
        private DWrite.Factory m_factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryHandlerDWrite"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="dxgiHandler">The dxgi handler.</param>
        internal FactoryHandlerDWrite(GraphicsCore core)
        {
            //Update member variables
            m_core = core;

            //Create DirectWrite Factory object
            m_factory = new DWrite.Factory(DWrite.FactoryType.Shared);
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        internal void UnloadResources()
        {
            m_factory = GraphicsHelper.DisposeObject(m_factory);
        }

        /// <summary>
        /// Gets the Factory object.
        /// </summary>
        internal DWrite.Factory Factory
        {
            get { return m_factory; }
        }

        /// <summary>
        /// Is DirectWrite initialized successfully?
        /// </summary>
        public bool IsInitialized
        {
            get { return m_factory != null; }
        }
    }
}
#endif