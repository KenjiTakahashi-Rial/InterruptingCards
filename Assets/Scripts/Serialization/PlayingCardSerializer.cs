using InterruptingCards.Models;

namespace InterruptingCards
{
    public struct SerializedPlayingCard
    {
        public PlayingCardSuit Suit;
        public PlayingCardRank Rank;
    }

    public class PlayingCardSerializer
    {
        public SerializedPlayingCard Serialize(PlayingCard card)
        {
            return new SerializedPlayingCard { Suit = card.Suit, Rank = card.Rank };
        }

        public PlayingCard Deserialize(SerializedPlayingCard card)
        {
            return new PlayingCard(card.Suit, card.Rank);
        }
    }
}

