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
using FrozenSky;
using FrozenSky.Multimedia.Objects;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Graphics
{
    public class CardObjectType : ObjectType
    {
        public override VertexStructure[] BuildStructure(StructureBuildOptions buildOptions)
        {
            VertexStructure[] result = new VertexStructure[2];
            
            result[0] = new VertexStructure();
            result[0].BuildRect4V(
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(-0.5f, 0f, 0.5f));
            result[0].Material = this.FrontMaterial;

            result[1] = new VertexStructure();
            result[1].BuildRect4V(
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f, -0.5f));
            result[1].Material = this.BackMaterial;

            return result;
        }

        public NamedOrGenericKey FrontMaterial
        {
            get;
            set;
        }

        public NamedOrGenericKey BackMaterial
        {
            get;
            set;
        }
    }
}
