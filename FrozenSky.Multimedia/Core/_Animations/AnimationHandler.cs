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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using FrozenSky.Util;

namespace FrozenSky.Multimedia.Core
{
    public class AnimationHandler : AnimationSequence
    {
        private object m_owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationHandler"/> class.
        /// </summary>
        /// <param name="owner">The owner object of this AnimationHandler.</param>
        public AnimationHandler(object owner)
        {
            m_owner = owner;
        }

        /// <summary>
        /// Starts building an animation sequence for the current target object.
        /// </summary>
        internal IAnimationSequenceBuilder<TargetObject> BuildAnimationSequence<TargetObject>()
            where TargetObject : class, IAnimatableObject
        {
            return new AnimationSequenceBuilder<TargetObject>(this);
        }

        /// <summary>
        /// Starts building an animation sequence for the current target object.
        /// </summary>
        /// <param name="animatedObject">The target object which is to be animated.</param>
        internal IAnimationSequenceBuilder<TargetObject> BuildAnimationSequence<TargetObject>(TargetObject animatedObject)
            where TargetObject : class
        {
            return new AnimationSequenceBuilder<TargetObject>(this, animatedObject);
        }

        /// <summary>
        /// Called when an animation throws an exception during execution.
        /// </summary>
        /// <param name="animation">The failed animation.</param>
        /// <param name="ex">The exception thrown.</param>
        protected override AnimationFailedReaction OnAnimationFailed(IAnimation animation, System.Exception ex)
        {
            return AnimationFailedReaction.RemoveAndContinue;
        }

        /// <summary>
        /// Gets the owner object.
        /// </summary>
        public object Owner
        {
            get { return m_owner; }
        }
    }
}