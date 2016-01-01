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
using SeeingSharp.Checking;
using SeeingSharp.Util;

namespace SeeingSharp.Samples.Base
{
    public class SampleDescription
    {
        #region data sources
        private SampleInfoAttribute m_attrib;
        private Type m_sampleClass;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDescription"/> class.
        /// </summary>
        /// <param name="attrib">The attribute.</param>
        /// <param name="sampleClass">The class behind the sample.</param>
        public SampleDescription(SampleInfoAttribute attrib, Type sampleClass)
        {
            attrib.EnsureNotNull("attrib");
            attrib.Category.EnsureNotNullOrEmpty("attrib.Category");
            attrib.Name.EnsureNotNullOrEmpty("attrib.Name");
            sampleClass.EnsureNotNull("sampleType");

            m_attrib = attrib;
            m_sampleClass = sampleClass;
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

        public Type SampleClass
        {
            get { return m_sampleClass; }
        }

        public SampleTargetPlatform TargetPlatform
        {
            get { return m_attrib.TargetPlatform; }
        }

        public ResourceLink ImageLink
        {
            get
            {
                return new AssemblyResourceUriBuilder(
                    "SeeingSharp.Samples.Base", false,
                    string.Format(
                        "Assets/SampleImages/{0}_{1}.png",
                        this.Category.Replace(' ', '_'),
                        this.Name.Replace(' ', '_')));
            }
        }
    }

}
