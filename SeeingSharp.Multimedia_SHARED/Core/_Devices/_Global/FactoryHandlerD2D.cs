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

//Some namespace mappings
using D2D = SharpDX.Direct2D1;

namespace SeeingSharp.Multimedia.Core
{
    public class FactoryHandlerD2D
    {
        #region Resources form Direct2D api
        private D2D.Factory m_factory;
        private D2D.Factory2 m_factory2;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryHandlerD2D"/> class.
        /// </summary>
        /// <param name="core">The core.</param>
        internal FactoryHandlerD2D(GraphicsCore core)
        {
            bool doFallbackMethod = core.Force2DFallbackMethod;

            // Do default method (Windows 8 and newer)
            if (!doFallbackMethod)
            {
                try
                {
                    m_factory2 = new D2D.Factory2(
                        D2D.FactoryType.SingleThreaded,
                        core.IsDebugEnabled ? D2D.DebugLevel.Information : D2D.DebugLevel.None);
                    m_factory = m_factory2;
                }
                catch (Exception) { doFallbackMethod = true; }
            }

            // Fallback method (on older windows platforms (< Windows 8))
            if (doFallbackMethod)
            {
                m_factory2 = null;
                m_factory = new D2D.Factory(
                    D2D.FactoryType.SingleThreaded,
                    core.IsDebugEnabled ? D2D.DebugLevel.Information : D2D.DebugLevel.None);
            }
        }

        /// <summary>
        /// Unloads all resources.
        /// </summary>
        internal void UnloadResources()
        {
            GraphicsHelper.SafeDispose(ref m_factory);
            m_factory2 = null;
        }

        /// <summary>
        /// Gets the factory object.
        /// </summary>
        internal D2D.Factory Factory
        {
            get { return m_factory; }
        }

        internal D2D.Factory2 Factory2
        {
            get { return m_factory2; }
        }

        /// <summary>
        /// Is Direct2D initialized?
        /// </summary>
        public bool IsInitialized
        {
            get { return m_factory2 != null; }
        }

        public bool IsUsingFallbackMethod
        {
            get { return (m_factory2 == null) && (m_factory != null); }
        }
    }
}