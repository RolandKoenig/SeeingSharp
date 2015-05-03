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

using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
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

        /// <summary>
        /// Indicates whether this single card is uncovered or not.
        /// </summary>
        public bool IsCardUncovered
        {
            get;
            set;
        }

        public CardPair Pair
        {
            get;
            private set;
        }

        public int CountRunningAnimations
        {
            get { return base.AnimationHandler.CountRunningAnimations; }
        }
    }
}
