using System;
using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Models
{
    public static class Utilities
    {
        public static ICard<S, R> Remove<S, R>(IList<ICard<S, R>> cards, S suit, R rank) where S : Enum where R : Enum
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (card.Suit.Equals(suit) && card.Rank.Equals(rank))
                {
                    cards.RemoveAt(i);
                    return card;
                }
            }

            throw new CardNotFoundException();
        }
    }
}
