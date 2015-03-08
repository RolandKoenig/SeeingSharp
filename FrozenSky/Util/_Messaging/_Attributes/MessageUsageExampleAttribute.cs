using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class MessageUsageExampleAttribute : Attribute
    {
        public MessageUsageExampleAttribute(string exampleString)
        {
            this.ExampleString = exampleString;
        }

        public string ExampleString
        {
            get;
            private set;
        }
    }
}
