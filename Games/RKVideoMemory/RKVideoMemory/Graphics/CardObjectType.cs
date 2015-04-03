using FrozenSky;
using FrozenSky.Multimedia.Objects;
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
            VertexStructure[] result = new VertexStructure[1];
            result[0] = new VertexStructure();



            return result;
        }
    }
}
