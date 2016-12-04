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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeeingSharp.Util
{
    public class SerializerRepository
    {
        private static SerializerRepository s_current;

        private ConcurrentDictionary<Type, XmlSerializer> m_serializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerRepository"/> class.
        /// </summary>
        private SerializerRepository()
        {
            m_serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        }

        /// <summary>
        /// Gets the xml serializer for the given type.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        public XmlSerializer GetSerializer<T>()
        {
            return GetSerializer(typeof(T));
        }

        /// <summary>
        /// Gets the xml serializer for the given type.
        /// </summary>
        /// <param name="serializerType">The requested type.</param>
        public XmlSerializer GetSerializer(Type serializerType)
        {
            return m_serializers.GetOrAdd(
                serializerType,
                (typeParam) => new XmlSerializer(typeParam));
        }

        /// <summary>
        /// Gets the current instance of the serializer collection.
        /// </summary>
        public static SerializerRepository Current
        {
            get
            {
                if (s_current == null) { s_current = new SerializerRepository(); }
                return s_current;
            }
        }
    }
}
