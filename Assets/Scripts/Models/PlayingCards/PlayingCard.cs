using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class PlayingCard : AbstractCard<PlayingCardSuit, PlayingCardRank>, INetworkSerializable
    {
        public PlayingCard(PlayingCardSuit suit, PlayingCardRank rank) : base(suit, rank) { }

        public override ICard<PlayingCardSuit, PlayingCardRank> Clone()
        {
            return new PlayingCard(Suit, Rank);
        }

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _suit);
            serializer.SerializeValue(ref _rank);
        }
    }
}