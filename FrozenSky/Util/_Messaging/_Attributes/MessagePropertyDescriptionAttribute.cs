using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class MessagePropertyDescriptionAttribute : Attribute
    {
        public MessagePropertyDescriptionAttribute(string description)
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
