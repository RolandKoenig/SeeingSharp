﻿#region License information (SeeingSharp and all based games/applications)
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

namespace SeeingSharp.Multimedia.Core
{
    /// <summary>
    /// Base class for all scene components.
    /// </summary>
    public abstract class SceneComponentBase
#if DESKTOP
        : IComponent, IDisposable
#endif
    {

#if DESKTOP
        event EventHandler IComponent.Disposed
        {
            add { }
            remove { }
        }
#endif

        internal abstract object AttachInternal(SceneManipulator manipulator, ViewInformation correspondingView);

        internal abstract void DetachInternal(SceneManipulator manipulator, ViewInformation correspondingView, object componentContext);

        internal abstract void UpdateInternal(SceneRelatedUpdateState updateState, ViewInformation correspondingView, object componentContext);

        /// <summary>
        /// If not null or empty, this property indicates which components to the same thing.
        /// If you attach a component to a scene where another component with the same group
        /// is active, this other component gets detached automatically.
        /// 
        /// This feature was developed initially for various camera controls which 
        /// should not be activ simultaneously.
        /// </summary>
#if DESKTOP
        [Browsable(false)]
#endif
        public virtual string ComponentGroup
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Is this component specific for one view?
        /// </summary>
#if DESKTOP
        [Browsable(false)]
#endif
        public virtual bool IsViewSpecific
        {
            get { return false; }
        }

#if DESKTOP
        ISite System.ComponentModel.IComponent.Site
        {
            get;
            set;
        }

        void IDisposable.Dispose()
        {

        }
#endif
    }
}
