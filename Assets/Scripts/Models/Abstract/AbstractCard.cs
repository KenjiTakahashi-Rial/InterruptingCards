using System;

using UnityEngine;

namespace InterruptingCards.Models.Abstract
{
    public abstract class AbstractCard<S, R> : MonoBehaviour, ICard<S, R> where S : Enum where R : Enum
    {
        protected AbstractCard(S suit, R rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public S Suit { get; }

        public R Rank { get; }
    }
}