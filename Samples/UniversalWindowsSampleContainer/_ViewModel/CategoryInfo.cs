#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
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
