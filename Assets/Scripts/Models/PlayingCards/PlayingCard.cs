using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class PlayingCard : AbstractCard<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCard(
            PlayingCardSuit suit = PlayingCardSuit.Invalid, PlayingCardRank rank = PlayingCardRank.Invalid
        ) : base(suit, rank) { }

        public override object Clone()
        {
            return new PlayingCard(Suit, Rank);
        }

        // TODO:
        //public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        //{
        //    serializer.SerializeValue(ref _suit);
        //    serializer.SerializeValue(ref _rank);
        //}
    }
}