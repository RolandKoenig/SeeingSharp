#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Samples.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SampleInfoAttribute : Attribute
    {
        public SampleInfoAttribute(
            string category, string name, int orderID, string codeUrl, 
            SampleTargetPlatform targetPlatform)
        {
            this.Category = category;
            this.Name = name;
            this.OrderID = orderID;
            this.CodeUrl = codeUrl;
            this.TargetPlatform = targetPlatform;
        }

        public string Category
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public int OrderID
        {
            get;
            private set;
        }

        public string CodeUrl
        {
            get;
            private set;
        }

        public SampleTargetPlatform TargetPlatform
        {
            get;
            private set;
        }
    }
}
