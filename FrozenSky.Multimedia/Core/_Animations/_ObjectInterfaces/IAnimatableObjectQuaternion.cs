using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public interface IAnimatableObjectQuaternion
    {
        /// <summary>
        /// Gets or sets the quaternion used for object rotation calculation.
        /// </summary>
        Quaternion RotationQuaternion
        {
            get;
            set;
        }
    }
}
