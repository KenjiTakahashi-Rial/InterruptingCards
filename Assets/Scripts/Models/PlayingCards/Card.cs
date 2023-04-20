using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Models.PlayingCards
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum Rank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
    }

    public class Card : ICard<Suit, Rank>
    {
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Suit Suit { get; }

        public Rank Rank { get; }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}