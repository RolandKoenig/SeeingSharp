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
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using RKVideoMemory.Data;
using RKVideoMemory.Util;
using RKVideoMemory.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    public class CardPair : SceneLogicalObject
    {
        private MemoryPairData m_pairData;
        private bool m_isUncovered;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPair"/> class.
        /// </summary>
        /// <param name="pairData">The pair data.</param>
        public CardPair(MemoryPairData pairData)
        {
            m_pairData = pairData;
            m_isUncovered = false;
        }

        /// <summary>
        /// Called when the main screen was entered.
        /// </summary>
        private void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            for(int loop=0 ; loop<this.Cards.Length; loop++)
            {
                Card actCard = this.Cards[loop];

                actCard.AnimationHandler.CancelAnimations();

                if(m_isUncovered)
                {
                    actCard.BuildAnimationSequence()
                        .MainScreen_WhenUncovered()
                        .Apply();  
                }
                else
                {
                    actCard.BuildAnimationSequence()
                        .MainScreen_WhenCovered()
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
    }
}
