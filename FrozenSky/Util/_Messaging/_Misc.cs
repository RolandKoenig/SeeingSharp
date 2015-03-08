using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public enum FrozenSkyMessageThreadingBehavior
    {
        /// <summary>
        /// Ignore threading. Just use default lock on registration list.
        /// </summary>
        Ignore,

        /// <summary>
        /// Ensures that the main thread of this MessageHandler is used on synchronous calls.
        /// </summary>
        EnsureMainThreadOnSyncCalls
    }
}
