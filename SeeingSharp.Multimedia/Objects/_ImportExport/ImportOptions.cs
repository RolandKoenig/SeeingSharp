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
using SeeingSharp.Multimedia.Core;

namespace SeeingSharp.Multimedia.Objects
{
    public class ImportOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportOptions"/> class.
        /// </summary>
        public ImportOptions()
        {
            this.ResourceCoordinateSystem = CoordinateSystem.LeftHanded_UpY;
            this.ResizeFactor = 1f;
        }

        /// <summary>
        /// Gets or sets the resize factor.
        /// This is needed to transform coordinate from one measure unit to another.
        /// </summary>
        public float ResizeFactor
        {
            get;
            set;
        }

        public CoordinateSystem ResourceCoordinateSystem
        {
            get;
            set;
        }

        /// <summary>
        /// Should triangle order be changes by the import logic?
        /// </summary>
        public bool ChangeTriangleOrder
        {
            get
            {
                switch (this.ResourceCoordinateSystem)
                {
                    case CoordinateSystem.LeftHanded_UpY:
                    case CoordinateSystem.RightHanded_UpZ:
                        return false;

                    case CoordinateSystem.LeftHanded_UpZ:
                    case CoordinateSystem.RightHanded_UpY:
                        return true;

                    default:
                        throw new SeeingSharpGraphicsException(string.Format(
                            "Unknown coordinate system {0}!",
                            this.ResourceCoordinateSystem));
                }
            }
        }

    }
}
