#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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

using SeeingSharp;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class CardObjectType : ObjectType
    {
        public override VertexStructure[] BuildStructure(StructureBuildOptions buildOptions)
        {
            float halfWidth = Constants.TILE_WIDTH / 2f;
            float halfHeight = Constants.TILE_HEIGHT / 2f;

            VertexStructure[] result = new VertexStructure[2];

            result[0] = new VertexStructure();
            result[0].BuildRect4V(
                new Vector3(-halfWidth, 0f, -halfHeight),
                new Vector3(halfWidth, 0f, -halfHeight),
                new Vector3(halfWidth, 0f, halfHeight),
                new Vector3(-halfWidth, 0f, halfHeight));
            result[0].Material = this.FrontMaterial;

            result[1] = new VertexStructure();
            result[1].BuildRect4V(
                new Vector3(-halfWidth, 0f, halfHeight),
                new Vector3(halfWidth, 0f, halfHeight),
                new Vector3(halfWidth, 0f, -halfHeight),
                new Vector3(-halfWidth, 0f, -halfHeight));
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
