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
        /// <param name="resGeometry">The key of the GeometryResource.</param>
        /// <param name="cardPair">The pair this card belongs to.</param>
        public Card(NamedOrGenericKey resGeometry, CardPair cardPair)
            : base(resGeometry)
        {
            this.Pair = cardPair;
        }



        public CardPair Pair
        {
            get;
            private set;
        }
    }
}
