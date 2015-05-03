#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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

//Some namespace mappings
using D2D = SharpDX.Direct2D1;

//Some type mappings
using D2dFactory = SharpDX.Direct2D1.Factory;

namespace SeeingSharp.Multimedia.Core
{
    public class FactoryHandlerD2D
    {
        private GraphicsCore m_core;

        //Resources form Direct2D api
        private D2dFactory m_factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryHandlerD2D"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        /// <param name="dxgiHandler">The dxgi handler.</param>
        internal FactoryHandlerD2D(GraphicsCore core)
        {
            //Update member variables
            m_core = core;

            //Create the factory object
            m_factory = new D2dFactory(
                D2D.FactoryType.SingleThreaded,
                core.IsDebugEnabled ? D2D.DebugLevel.Information : D2D.DebugLevel.None);
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        internal void UnloadResources()
        {
            m_factory = GraphicsHelper.DisposeObject(m_factory);
        }

        /// <summary>
        /// Gets the factory object.
        /// </summary>
        internal D2D.Factory Factory
        {
            get { return m_factory; }
        }

        /// <summary>
        /// Is Direct2D initialized?
        /// </summary>
        public bool IsInitialized
        {
            get { return m_factory != null; }
        }
    }
}