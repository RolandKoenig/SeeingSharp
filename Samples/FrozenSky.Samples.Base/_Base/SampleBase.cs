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
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Samples.Base
{
    public abstract class SampleBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBase"/> class.
        /// </summary>
        public SampleBase()
        {

        }

        public void SetClosed()
        {
            if(!this.IsClosed)
            {
                this.IsClosed = true;
                this.OnClosed();
            }
        }

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        /// <param name="targetRenderLoop">The target render loop.</param>
        public abstract Task OnStartup(RenderLoop targetRenderLoop);

        /// <summary>
        /// Called when the sample is closed.
        /// Scene objects and resources are automatically removed, no need to do it
        /// manually in this method's implementation.
        /// </summary>
        public virtual void OnClosed()
        {

        }

        public bool IsClosed
        {
            get;
            private set;
        }
    }
}
