using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using RKVideoMemory.Data;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPair"/> class.
        /// </summary>
        /// <param name="pairData">The pair data.</param>
        public CardPair(MemoryPairData pairData)
        {
            m_pairData = pairData;
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
                //actCard.BuildAnimationSequence()
                //    .Rot
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
    }
}
