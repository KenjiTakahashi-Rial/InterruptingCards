using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Utilities
{
    public static class Utilities
    {
        public static ICard Remove(IList<ICard> cards, int cardId)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (cardId == card.Id)
                {
                    cards.RemoveAt(i);
                    return card;
                }
            }

            throw new CardNotFoundException();
        }
    }
}
