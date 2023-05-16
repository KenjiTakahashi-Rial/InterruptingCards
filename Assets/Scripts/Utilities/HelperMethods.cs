using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Utilities
{
    public static class HelperMethods
    {
        public static Card Remove(IList<Card> cards, int cardId)
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
