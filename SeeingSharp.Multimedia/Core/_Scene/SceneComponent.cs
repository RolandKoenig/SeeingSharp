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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// A base class for components which we can easily attach to a 
    /// renderer element. They can be attached directly from Xaml code.
    /// </summary>
    public abstract class SceneComponent<T> : SceneComponentBase
        where T : class
    {
        internal override object AttachInternal(SceneManipulator manipulator)
        {
            return this.Attach(manipulator);
        }

        internal override void DetachInternal(SceneManipulator manipulator, object componentContext)
        {
            T componentContextCasted = componentContext as T;
            componentContextCasted.EnsureNotNull(nameof(componentContext));

            this.Detach(manipulator, componentContextCasted);
        }

        /// <summary>
        /// Attaches this component to a scene.
        /// Be careful, this method gets called from a background thread of seeing#!
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene we attach to.</param>
        protected abstract T Attach(SceneManipulator manipulator);

        /// <summary>
        /// Detaches this component from a scene.
        /// Be careful, this method gets called from a background thread of seeing#!
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene we attach to.</param>
        /// <param name="componentContext">A context variable containing all createded objects during call of Attach.</param>
        protected abstract void Detach(SceneManipulator manipulator, T componentContext);
    }
}
