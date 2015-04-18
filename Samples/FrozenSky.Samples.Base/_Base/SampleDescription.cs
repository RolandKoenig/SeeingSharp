#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using FrozenSky.Checking;
using FrozenSky.Util;

namespace FrozenSky.Samples.Base
{
    public class SampleDescription
    {
        private SampleInfoAttribute m_attrib;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDescription"/> class.
        /// </summary>
        /// <param name="attrib">The attribute.</param>
        public SampleDescription(SampleInfoAttribute attrib)
        {
            attrib.EnsureNotNull("attrib");
            attrib.Category.EnsureNotNullOrEmpty("attrib.Category");
            attrib.Name.EnsureNotNullOrEmpty("attrib.Name");

            m_attrib = attrib;        
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Category, this.Name);
        }

        public string Category
        {
            get { return m_attrib.Category; }
        }

        public string Name
        {
            get { return m_attrib.Name; }
        }

        public int OrderID
        {
            get { return m_attrib.OrderID; }
        }

        public string CodeUrl
        {
            get { return m_attrib.CodeUrl; }
        }

        public ResourceLink ImageLink
        {
            get
            {
                return new AssemblyResourceUriBuilder(
                    "FrozenSky.Samples.Base", false,
                    string.Format(
                        "Assets/SampleImages/{0}_{1}.png",
                        this.Category.Replace(' ', '_'),
                        this.Name.Replace(' ', '_')));
            }
        }
    }

}
