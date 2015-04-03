using FrozenSky.Multimedia.Objects;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class Card : GenericObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// </summary>
        public Card()
            : base(NamedOrGenericKey.Empty)
        {

        }

        public CardPair Pair
        {
            get;
            set;
        }
    }
}
