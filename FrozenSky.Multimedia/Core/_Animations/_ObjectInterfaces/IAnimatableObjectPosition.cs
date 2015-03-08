using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    public interface IAnimatableObjectPosition 
    {
        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        Vector3 Position
        {
            get;
            set;
        }
    }
}
