using System.Collections.Generic;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public static class Utilities
    {
        public static ICard Remove(IList<ICard> cards, CardSuit suit, CardRank rank)
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
