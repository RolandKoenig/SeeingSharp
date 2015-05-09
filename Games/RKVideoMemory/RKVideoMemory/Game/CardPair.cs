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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using RKVideoMemory.Data;
using RKVideoMemory.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class CardPair : SceneLogicalObject
    {
        private CardPairData m_pairData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPair"/> class.
        /// </summary>
        /// <param name="pairData">The pair data.</param>
        public CardPair(CardPairData pairData)
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
                    .ChangeOpacityTo(0f, TimeSpan.FromMilliseconds(300))
                    .Apply();
            }
        }

        /// <summary>
        /// Called when the main screen was entered.
        /// </summary>
        private void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            for(int loop=0 ; loop<this.Cards.Length; loop++)
            {
                Card actCard = this.Cards[loop];

                // Cancel current animations
                actCard.AnimationHandler.CancelAnimations();

                if(this.IsUncovered)
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
        /// Called when the main game loop sends a cyclic trigger.
        /// This one should start some shaking animation as long as nothin else happens on the screen.
        /// </summary>
        private void OnMessage_Received(GameTriggerMessage message)
        {
            for (int loop = 0; loop < this.Cards.Length; loop++)
            {
                Card actCard = this.Cards[loop];
                if (actCard.AnimationHandler.CountRunningAnimations > 0) { continue; }

                // Trigger 'shaking' animation
                if (ThreadSafeRandom.Next(0, 100) < 10)
                {

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
