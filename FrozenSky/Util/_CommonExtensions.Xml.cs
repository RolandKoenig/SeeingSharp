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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FrozenSky.Util
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
        public static Vector3 ReadContentAsVector3(this XmlReader xmlReader, IFormatProvider formatProvider)
        {
            string[] components = xmlReader.ReadContentAsString().Split(',');
            if (components.Length != 3) { throw new FrozenSkyException("Invalid vector3 format in xml file!"); }

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
        public static Vector3 ReadElementContentAsVector3(this XmlReader xmlReader)
        {
            return ReadElementContentAsVector3(xmlReader, CULTURE_EN.NumberFormat);
        }

        /// <summary>
        /// Reads a vector from the given xml reader.
        /// </summary>
        /// <param name="xmlReader">The xml reader.</param>
        public static Vector3 ReadElementContentAsVector3(this XmlReader xmlReader, IFormatProvider formatProvider)
        {
            string[] components = xmlReader.ReadElementContentAsString().Split(',');
            if (components.Length != 3) { throw new FrozenSkyException("Invalid vector3 format in xml file!"); }

            Vector3 result = new Vector3();
            result.X = float.Parse(components[0], formatProvider);
            result.Y = float.Parse(components[1], formatProvider);
            result.Z = float.Parse(components[2], formatProvider);

            return result;
        }
    }
}
