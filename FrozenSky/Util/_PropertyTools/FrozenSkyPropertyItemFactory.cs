using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public class FrozenSkyPropertyItemFactory : DefaultPropertyItemFactory
    {
        protected override string GetDisplayName(PropertyDescriptor pd, Type declaringType)
        {
            string displayName = base.GetDisplayName(pd, declaringType);
            displayName = displayName + ": ";
            return displayName;
        }
    }
}
