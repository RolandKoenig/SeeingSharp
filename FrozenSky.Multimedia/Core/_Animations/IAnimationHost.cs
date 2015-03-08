using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    /// <summary>
    /// A base interface for each objects which is able to manage animations.
    /// </summary>
    public interface IAnimationHost
    {
        /// <summary>
        /// Gets the animation handler of this object.
        /// </summary>
        AnimationHandler AnimationHandler
        {
            get;
        }
    }
}
