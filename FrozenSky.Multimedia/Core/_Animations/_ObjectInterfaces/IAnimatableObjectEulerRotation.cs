using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public interface IAnimatableObjectEulerRotation
    {
        /// <summary>
        /// Gets the or sets the euler rotation vector.
        /// </summary>
        Vector3 RotationEuler
        {
            get;
            set;
        }
    }
}
