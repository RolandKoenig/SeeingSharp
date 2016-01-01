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

namespace SeeingSharp.Infrastructure
{
    public class ExceptionInfoNode
    {
        private List<ExceptionInfoNode> m_innerExceptionNodes;
        private List<ExceptionProperty> m_properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfoNode"/> class.
        /// </summary>
        /// <param name="exceptionType">The type of the exception.</param>
        public ExceptionInfoNode(Type exceptionType)
        {
            m_innerExceptionNodes = new List<ExceptionInfoNode>();
            m_properties = new List<ExceptionProperty>();

            this.TypeName = exceptionType.Name;           
        }

        /// <summary>
        /// Gets or sets the name of this node.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection containing all subnodes.
        /// </summary>
        public List<ExceptionInfoNode> InnerExceptionNodes
        {
            get { return m_innerExceptionNodes; }
        }

        /// <summary>
        /// Gets a list containing all exception properties.
        /// </summary>
        public List<ExceptionProperty> Properties
        {
            get { return m_properties; }
        }
    }
}
