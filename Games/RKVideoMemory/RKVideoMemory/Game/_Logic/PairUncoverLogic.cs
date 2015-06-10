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
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Game
{
    /// <summary>
    /// This component is responsible for uncovering the cards by the player. 
    /// </summary>
    public class PairUncoverLogic : SceneLogicalObject
    {
        private List<Card> m_uncoveredCards;
        private bool m_currentlyUncovering;

        /// <summary>
        /// Initializes a new instance of the <see cref="PairUncoverLogic"/> class.
        /// </summary>
        public PairUncoverLogic()
        {
            m_uncoveredCards = new List<Card>();
        }

        /// <summary>
        /// Called when the player enters the main memory screen.
        /// </summary>
        private void OnMessage_Received(MainMemoryScreenEnteredMessage message)
        {
            m_currentlyUncovering = false;
            m_uncoveredCards.Clear();
        }

        /// <summary>
        /// Called when an object was clicked.
        /// </summary>
        private async void OnMessage_Received(ObjectsClickedMessage message)
        {
            Card selectedCard = message.ClickedObjects
                .FirstOrDefault() as Card;
            if (selectedCard == null) { return; }
            if (selectedCard.CountRunningAnimations > 0) { return; }
            if (selectedCard.IsCardUncovered) { return; }
            if (selectedCard.Pair.IsUncovered) { return; }
            if (m_currentlyUncovering) { return; }

            // Perform uncover animation
            m_currentlyUncovering = true;
            try
            {
                selectedCard.AnimationHandler.CancelAnimations();
                await selectedCard.BuildAnimationSequence()
                    .MainScreen_PerformUncover()
                    .ApplyAsync();

                selectedCard.IsCardUncovered = true;
            }
            finally
            {
                m_currentlyUncovering = false;
            }

            // Notify uncovered card
            this.Messenger.Publish(new CardUncoveredByPlayerMessage(selectedCard));
        }

        /// <summary>
        /// Called when a single card was uncovered by the player.
        /// </summary>
        private async void OnMessage_Received(CardUncoveredByPlayerMessage message)
        {
            m_uncoveredCards.EnsureDoesNotContain(message.Card, "m_uncoveredCards");
            m_uncoveredCards.EnsureCountInRange(0, 1, "m_uncoveredCards");

            m_uncoveredCards.Add(message.Card);
            if (m_uncoveredCards.Count < 2) { return; }

            // Select correspondig pairs of uncovered cards
            CardPairLogic[] pairs = m_uncoveredCards
                .Select((actCard) => actCard.Pair)
                .Distinct()
                .ToArray();
            pairs.EnsureCountInRange(1, 2, "pairs");

            m_currentlyUncovering = true;
            try
            {
                if (pairs.Length == 1)
                {
                    // Yeah, we found a correct pair! Trigger movie
                    CardPairLogic selectedPair = pairs[0];
                    selectedPair.IsUncovered = true;

                    this.Messenger.Publish(new CardPairUncoveredByPlayerMessage(selectedPair));
                }
                else
                {
                    // Bad.. bad try, trigger uncovering of the cards
                    await Task.Delay(500);

                    Task[] recoverTasks = new Task[m_uncoveredCards.Count];
                    for(int loop=0 ; loop<m_uncoveredCards.Count; loop++)
                    {
                        Card actCard = m_uncoveredCards[loop];
                        actCard.IsCardUncovered = false;

                        recoverTasks[loop] = actCard.BuildAnimationSequence()
                            .MainScreen_PerformCover()
                            .ApplyAsync();
                    }
                    await Task.WhenAll(recoverTasks);
                }
            }
            finally
            {
                m_uncoveredCards.Clear();
                m_currentlyUncovering = false;
            }
        }
    }
}
