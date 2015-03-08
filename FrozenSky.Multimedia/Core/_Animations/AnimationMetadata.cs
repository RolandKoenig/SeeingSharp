#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public class AnimationMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationMetadata"/> class.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="finishedCallback">The finished callback.</param>
        public AnimationMetadata(IAnimation animation, Action finishedCallback)
        {
            this.Animation = animation;
            this.FinishedCallback = finishedCallback;
        }

        /// <summary>
        /// Gets the animation.
        /// </summary>
        public IAnimation Animation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the finished callback.
        /// </summary>
        public Action FinishedCallback
        {
            get;
            private set;
        }
    }
}
