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
using System.Reflection;
using SeeingSharp.Checking;

namespace SeeingSharp.Infrastructure
{
    public class ExceptionInfoNode : IComparable<ExceptionInfoNode>
    {
        private Exception m_exception;
        private List<ExceptionInfoNode> m_childNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfoNode"/> class.
        /// </summary>
        public ExceptionInfoNode(Exception ex)
        {
            ex.EnsureNotNull(nameof(ex));

            m_exception = ex;

            m_childNodes = new List<ExceptionInfoNode>();
            this.PropertyName = ex.GetType().GetTypeInfo().Name;
            this.PropertyValue = ex.Message;
        }

        public ExceptionInfoNode(ExceptionProperty property)
        {
            property.EnsureNotNull(nameof(property));

            m_childNodes = new List<ExceptionInfoNode>();
            this.PropertyName = property.Name;
            this.PropertyValue = property.Value;
        }

        public int CompareTo(ExceptionInfoNode other)
        {
            if(this.IsExceptionNode != other.IsExceptionNode)
            {
                if (this.IsExceptionNode) { return -1; }
                else { return 1; }
            }

            return this.PropertyName.CompareTo(other.PropertyName);
        }

        /// <summary>
        /// Gets a collection containing all subnodes.
        /// </summary>
        public List<ExceptionInfoNode> ChildNodes
        {
            get { return m_childNodes; }
        }

        public bool IsExceptionNode => m_exception != null;

        public string PropertyName { get; private set; }

        public string PropertyValue { get; private set; }
    }
}
