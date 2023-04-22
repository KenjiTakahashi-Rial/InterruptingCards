using InterruptingCards.Models;

namespace InterruptingCards.Serialization
{
    public struct SerializedPlayingCard
    {
        internal PlayingCardSuit Suit;
        internal PlayingCardRank Rank;
    }

    public class PlayingCardSerializer : ISerializer<SerializedPlayingCard, PlayingCard>
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

