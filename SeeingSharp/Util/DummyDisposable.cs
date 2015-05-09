﻿#region License information (SeeingSharp and all based games/applications)
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

using System;

namespace SeeingSharp.Util
{
    /// <summary>
    /// Dummy class that implements IDisposable.
    /// </summary>
    public class DummyDisposable : IDisposable
    {
        private Action m_onDispsoeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyDisposable"/> class.
        /// </summary>
        /// <param name="onDisposeAction">The action to call on Dispose.</param>
        public DummyDisposable(Action onDisposeAction)
        {
            m_onDispsoeAction = onDisposeAction;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_onDispsoeAction != null) { m_onDispsoeAction(); }
        }
    }
}