using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    /// <summary>
    /// This enum describes the threading behavior of a MessageHandler.
    /// </summary>
    public enum FrozenSkyMessageThreadingBehavior
    {
        /// <summary>
        /// Ignore threading. Just use default lock on registration list.
        /// </summary>
        Ignore,

        /// <summary>
        /// Ensures that the main thread of this MessageHandler is used on synchronous calls.
        /// </summary>
        EnsureMainThreadOnSyncCalls,

        /// <summary>
        /// Ensures that the main SynchronizationContext is set when 
        /// this MessageHandler is used on synchronous calls.
        /// </summary>
        EnsureMainSyncContextOnSyncCalls
    }
}
