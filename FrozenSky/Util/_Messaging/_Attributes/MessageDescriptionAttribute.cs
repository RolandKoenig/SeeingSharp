using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class MessageDescriptionAttribute : Attribute
    {
        public MessageDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}
