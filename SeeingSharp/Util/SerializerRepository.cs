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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        /// <summary>
        /// Gets the default JsonSerializer which can be used to serialize complex object graphs.
        /// The serializer should be threadsafe, see http://json.codeplex.com/discussions/110461
        /// </summary>
        public static readonly JsonSerializer DEFAULT_JSON;
        public static readonly JsonSerializer DEFAULT_JSON_WITHOUT_INDENT;

        private static SerializerRepository s_current;

        private ConcurrentDictionary<Type, XmlSerializer> m_serializers;

        /// <summary>
        /// Initializes the <see cref="SerializerRepository"/> class.
        /// </summary>
        static SerializerRepository()
        {
            // Create the default serializer
            DEFAULT_JSON = new JsonSerializer();
            DEFAULT_JSON.ContractResolver = new SeeingSharpJsonContractResolver();
            DEFAULT_JSON.PreserveReferencesHandling = PreserveReferencesHandling.All;
            DEFAULT_JSON.Formatting = Formatting.Indented;
            DEFAULT_JSON.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            DEFAULT_JSON.TypeNameHandling = TypeNameHandling.Auto;
            DEFAULT_JSON.MissingMemberHandling = MissingMemberHandling.Ignore;

            // Create the default serializer without any indent
            DEFAULT_JSON_WITHOUT_INDENT = new JsonSerializer();
            DEFAULT_JSON_WITHOUT_INDENT.ContractResolver = new SeeingSharpJsonContractResolver();
            DEFAULT_JSON_WITHOUT_INDENT.PreserveReferencesHandling = PreserveReferencesHandling.All;
            DEFAULT_JSON_WITHOUT_INDENT.Formatting = Formatting.None;
            DEFAULT_JSON_WITHOUT_INDENT.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            DEFAULT_JSON_WITHOUT_INDENT.TypeNameHandling = TypeNameHandling.Auto;
            DEFAULT_JSON_WITHOUT_INDENT.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

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

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Base class for creating a custom contract resolver that looks at each property to see if
        /// it is allowed to be serialized.
        /// </summary>
        public class SeeingSharpJsonContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// Gets the serializable members for the type.
            /// </summary>
            /// <param name="objectType">The type to get serializable members for.</param>
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                // Call base class and return this result in some cases
                //  e. g. => Don't modify here when we have a value type (like Vector3, Color, DateTime, TimeSpan, ...)
                List<MemberInfo> result = base.GetSerializableMembers(objectType);
                if (result == null) { return result; }

                List<MemberInfo> trueResult = new List<MemberInfo>(result.Count);
                if (objectType.GetTypeInfo().IsValueType)
                {
                    // Filter out all members which only have get-access
                    foreach(MemberInfo actMember in result)
                    {
                        PropertyInfo actPropertyInfo = actMember as PropertyInfo;
                        if((actPropertyInfo != null) &&
                           (actPropertyInfo.SetMethod == null))
                        {
                            continue;
                        }

                        trueResult.Add(actMember);
                    }
                }
                else
                {
                    // Filter out all members which don't have a JsonProperty attribute attached
                    foreach (MemberInfo actMember in result)
                    {
                        JsonPropertyAttribute actPropertyAttrib = actMember.GetCustomAttribute<JsonPropertyAttribute>();
                        if (actPropertyAttrib == null) { continue; }

                        trueResult.Add(actMember);
                    }
                }

                return trueResult;
            }
        }
    }
}
