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
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Util
{
    public static class RKRocketSerializerUtil
    {
        /// <summary>
        /// Gets the default JsonSerializer which can be used to serialize complex object graphs.
        /// The serializer should be threadsafe, see http://json.codeplex.com/discussions/110461
        /// </summary>
        public static readonly JsonSerializer DEFAULT_JSON;
        public static readonly JsonSerializer DEFAULT_JSON_WITHOUT_INDENT;

        /// <summary>
        /// Gets the default encoding for scenarios where 1 character should be mapped on 1 byte fixed.
        /// </summary>
        public static readonly Encoding ENCODING_DEFAULT_1BYTE = Encoding.ASCII;
        /// <summary>
        /// Gets the default encoding for scenarios where 1 character should be mapped to 2 bytes fixed.
        /// </summary>
        public static readonly Encoding ENCODING_DEFAULT_2BYTE = Encoding.Unicode;

        /// <summary>
        /// Gets the default encoding for scenarios where 1 character should only be mapped to 2 bytes if needed so (on special characters).
        /// </summary>
        public static readonly Encoding ENCODING_DEFAULT_FLEXIBLE = Encoding.UTF8;

        /// <summary>
        /// Initializes the <see cref="SerializerCollection"/> class.
        /// </summary>
        static RKRocketSerializerUtil()
        {
            // Create the default serializer
            DEFAULT_JSON = new JsonSerializer();
            DEFAULT_JSON.ContractResolver = new CustomJsonContractResolver();
            DEFAULT_JSON.PreserveReferencesHandling = PreserveReferencesHandling.All;
            DEFAULT_JSON.Formatting = Formatting.Indented;
            DEFAULT_JSON.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            DEFAULT_JSON.TypeNameHandling = TypeNameHandling.Auto;
            DEFAULT_JSON.MissingMemberHandling = MissingMemberHandling.Ignore;

            // Create the default serializer without any indent
            DEFAULT_JSON_WITHOUT_INDENT = new JsonSerializer();
            DEFAULT_JSON_WITHOUT_INDENT.ContractResolver = new CustomJsonContractResolver();
            DEFAULT_JSON_WITHOUT_INDENT.PreserveReferencesHandling = PreserveReferencesHandling.All;
            DEFAULT_JSON_WITHOUT_INDENT.Formatting = Formatting.None;
            DEFAULT_JSON_WITHOUT_INDENT.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            DEFAULT_JSON_WITHOUT_INDENT.TypeNameHandling = TypeNameHandling.Auto;
            DEFAULT_JSON_WITHOUT_INDENT.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        /// <summary>
        /// Deserializes the json file.
        /// Throws an exception if the file is invalid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">The resource to be loaded.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <returns></returns>
        public static async Task<T> DeserializeJsonFromResourceAsync<T>(ResourceLink resource, JsonSerializer jsonSerializer = null, Encoding encoding = null)
            where T : class
        {
            Encoding encodingToUse = encoding ?? ENCODING_DEFAULT_FLEXIBLE;
            JsonSerializer jsonSerializerToUse = jsonSerializer ?? DEFAULT_JSON;

            using (Stream inStream = await resource.OpenInputStreamAsync())
            using (StreamReader streamReader = new StreamReader(inStream, encodingToUse))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                return await Task.Run(() => jsonSerializerToUse.Deserialize<T>(jsonReader));
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Base class for creating a custom contract resolver that looks at each property to see if
        /// it is allowed to be serialized.
        /// </summary>
        public class CustomJsonContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// Gets the serializable members for the type.
            /// </summary>
            /// <param name="objectType">The type to get serializable members for.</param>
            /// <returns>
            /// The serializable members for the type.
            /// </returns>
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                // Call base class and return this result in some cases
                //  e. g. => Don't modify here when we have a value type (like Vector3, Color, DateTime, TimeSpan, ...)
                List<MemberInfo> result = base.GetSerializableMembers(objectType);
                if (result == null) { return result; }
                if (objectType.GetTypeInfo().IsValueType) { return result; }

                // Filter out all members which don't have a JsonProperty attribute attached
                List<MemberInfo> trueResult = new List<MemberInfo>(result.Count);
                foreach (MemberInfo actMember in result)
                {
                    JsonPropertyAttribute actPropertyAttrib = actMember.GetCustomAttribute<JsonPropertyAttribute>();
                    if (actPropertyAttrib == null) { continue; }

                    trueResult.Add(actMember);
                }

                return trueResult;
            }
        }
    }
}
