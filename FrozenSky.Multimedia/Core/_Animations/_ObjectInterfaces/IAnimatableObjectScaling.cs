using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public interface IAnimatableObjectScaling
    {
        /// <summary>
        /// Gets or sets the scaling value of the object.
        /// </summary>
        Vector3 Scaling
        {
            get;
            set;
        }
    }
}
