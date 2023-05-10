using InterruptingCards.Models;

namespace InterruptingCards.Config
{
    public static class CardConfig
    {
        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        public static int CardId(ICard card)
        {
            return ((int)card.Suit & SuitBitMask) | ((int)card.Rank & RankBitMask);
        }
    }
}