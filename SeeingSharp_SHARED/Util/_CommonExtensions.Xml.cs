#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Numerics;

namespace SeeingSharp.Util
{
    public static partial class _CommonExtensions
    {
        private static readonly CultureInfo CULTURE_EN = new CultureInfo("en-GB");

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        public static Vector3 ReadContentAsVector3(this XmlReader xmlReader)
        {
            return ReadContentAsVector3(xmlReader, CULTURE_EN.NumberFormat);
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> for parsing <see cref="System.Single"/> values.</param>
        public static Vector3 ReadContentAsVector3(this XmlReader xmlReader, IFormatProvider formatProvider)
        {
            string[] components = xmlReader.ReadContentAsString().Split(',');
            if (components.Length != 3) { throw new SeeingSharpException("Invalid vector3 format in xml file!"); }

            Vector3 result = new Vector3();
            result.X = float.Parse(components[0], formatProvider);
            result.Y = float.Parse(components[1], formatProvider);
            result.Z = float.Parse(components[2], formatProvider);

            return result;
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        public static Vector2 ReadContentAsVector2(this XmlReader xmlReader)
        {
            return ReadContentAsVector2(xmlReader, CULTURE_EN.NumberFormat);
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> for parsing <see cref="System.Single"/> values.</param>
        public static Vector2 ReadContentAsVector2(this XmlReader xmlReader, IFormatProvider formatProvider)
        {
            string[] components = xmlReader.ReadContentAsString().Split(',');
            if (components.Length != 2) { throw new SeeingSharpException("Invalid vector2 format in xml file!"); }

            Vector2 result = new Vector2();
            result.X = float.Parse(components[0], formatProvider);
            result.Y = float.Parse(components[1], formatProvider);

            return result;
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        public static Vector3 ReadElementContentAsVector3(this XmlReader xmlReader)
        {
            return ReadElementContentAsVector3(xmlReader, CULTURE_EN.NumberFormat);
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> for parsing <see cref="System.Single"/> values.</param>
        public static Vector3 ReadElementContentAsVector3(this XmlReader xmlReader, IFormatProvider formatProvider)
        {
            string[] components = xmlReader.ReadElementContentAsString().Split(',');
            if (components.Length != 3) { throw new SeeingSharpException("Invalid vector3 format in xml file!"); }

            Vector3 result = new Vector3();
            result.X = float.Parse(components[0], formatProvider);
            result.Y = float.Parse(components[1], formatProvider);
            result.Z = float.Parse(components[2], formatProvider);

            return result;
        }
    }
}
