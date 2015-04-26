#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using SeeingSharp.Util;
using System.Reflection;

namespace SeeingSharp.Multimedia.Objects
{
    public class MaterialProperties
    {
        private NamedOrGenericKey m_key;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialProperties" /> class.
        /// </summary>
        public MaterialProperties()
        {
            m_key = NamedOrGenericKey.Empty;
        }

        public MaterialProperties Clone()
        {
            return base.MemberwiseClone() as MaterialProperties;
        }

        /// <summary>
        /// Gets or sets the resource source assembly.
        /// </summary>
        public Assembly ResourceSourceAssembly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original source of this geometry / material.
        /// </summary>
        public ResourceLink ResourceLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the material.
        /// </summary>
        public NamedOrGenericKey Key
        {
            get { return m_key; }
            set
            {
                if (value == null) { m_key = NamedOrGenericKey.Empty; }
                else { m_key = value; }
            }
        }

        /// <summary>
        /// Gets or sets the path to the texture.
        /// </summary>
        public NamedOrGenericKey TextureKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the diffuse color component of this material.
        /// </summary>
        public Color4 DiffuseColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ambient color component.
        /// </summary>
        public Color4 AmbientColor
        {
            get;
            set;
        }

        public Color4 EmissiveColor
        {
            get;
            set;
        }

        public Color4 Specular
        {
            get;
            set;
        }

        public float Shininess
        {
            get;
            set;
        }
    }
}