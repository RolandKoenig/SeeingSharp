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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FrozenSky.Multimedia.Drawing3D
{
    /// <summary>
    /// This class represents a specific viewpoint within the 3D world.
    /// </summary>
    [XmlType]
    public class Camera3DViewPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera3DViewPoint"/> class.
        /// </summary>
        [JsonConstructor]
        public Camera3DViewPoint()
        {
            this.OrthographicZoomFactor = 10f;
            this.CameraType = Camera3DType.Perspective;
        }

        [XmlIgnore]
        public Camera3DType CameraType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CameraType in string form.
        /// </summary>
        [XmlAttribute]
        [JsonProperty]
        public string CameraTypeString
        {
            get { return this.CameraType.ToString(); }
            set
            {
                Camera3DType valueParsed = Camera3DType.Perspective;
                if (Enum.TryParse(value, out valueParsed))
                {
                    this.CameraType = valueParsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the ViewPoint.
        /// </summary>
        [XmlElement]
        [JsonProperty]
        public Vector3 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rotation of the ViewPoint.
        /// </summary>
        [XmlElement]
        [JsonProperty]
        public Vector2 Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom factor if we have a orthographic camera.
        /// </summary>
        [XmlAttribute]
        [JsonProperty]
        public float OrthographicZoomFactor
        {
            get;
            set;
        }
    }
}
