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
