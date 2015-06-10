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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RKVideoMemory.Data;
using RKVideoMemory.Util;
using SeeingSharp;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;

namespace RKVideoMemory.Game
{
    public class CardPairLogic : SceneLogicalObject
    {
        private CardPairData m_pairData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPairLogic"/> class.
        /// </summary>
        /// <param name="pairData">The pair data.</param>
        public CardPairLogic(CardPairData pairData)
        {
            m_pairData = pairData;
            this.IsUncovered = false;
        }

        /// <summary>
        /// Called when [message_ received].
        /// </summary>
        private void OnMessage_Received(CardPairUncoveredByPlayerMessage message)
        {
            for (int loop = 0; loop < this.Cards.Length; loop++)
            {
                Card actCard = this.Cards[loop];
                actCard.AnimationHandler.CancelAnimations();
                actCard.BuildAnimationSequence()
                    .MainScreenLeave()
                    .Apply();
            }
        }

        /// <summary>
        /// Called when the main screen was entered.
        /// </summary>
        private void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            for (int loop = 0; loop < this.Cards.Length; loop++)
            {
                Card actCard = this.Cards[loop];

                // Cancel current animations
                actCard.AnimationHandler.CancelAnimations();

                if (this.IsUncovered)
                {
                    actCard.BuildAnimationSequence()
                        .MainScreenStart_WhenUncovered()
                        .Apply();
                }
                else
                {
                    actCard.BuildAnimationSequence()
                        .MainScreenStart_WhenCovered()
                        .Apply();
                }
            }
        }

        /// <summary>
        /// Was this cardpair found previously?
        /// </summary>
        public bool WasFound
        {
            get;
            set;
        }

        public Card[] Cards
        {
            get;
            set;
        }

        public bool IsUncovered
        {
            get;
            set;
        }

        public CardPairData PairData
        {
            get { return m_pairData; }
        }
    }
}