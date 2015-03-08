using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public class MessagePossibleSourceAttribute : Attribute
    {
        public MessagePossibleSourceAttribute(params string[] possibleSourceThreads)
        {
            this.PossibleSourceThreads = possibleSourceThreads;
        }

        public string[] PossibleSourceThreads
        {
            get;
            private set;
        }
    }
}