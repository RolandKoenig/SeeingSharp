using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalWindowsSampleContainer
{
    public class CategoryInfo
    {
        public static readonly IEqualityComparer<CategoryInfo> EqualityComparer = new HelperEqualityComparer();

        public CategoryInfo(string name, string iconSymbol)
        {
            this.Name = name;
            this.IconSymbol = iconSymbol;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Name { get; private set; } 
        public string IconSymbol { get; private set; }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class HelperEqualityComparer : IEqualityComparer<CategoryInfo>
        {
            public bool Equals(CategoryInfo x, CategoryInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(CategoryInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
