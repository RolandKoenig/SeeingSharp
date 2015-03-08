using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public class MessageAsyncRoutingTargetsAttribute : Attribute
    {
        public MessageAsyncRoutingTargetsAttribute(params string[] asyncTargetThreads)
        {
            this.AsyncTargetThreads = asyncTargetThreads;
        }

        public string[] AsyncTargetThreads
        {
            get;
            private set;
        }
    }
}