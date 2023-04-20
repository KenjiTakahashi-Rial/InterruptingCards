using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Models.PlayingCards
{
    public class Player : IPlayer<Suit, Rank>
    {
        public Player(string name, IHand<Suit, Rank> hand = null)
        {
            Name = name;
            hand ??= new Hand();
            Hand = hand;
        }

        public string Name { get; }

        public IHand<Suit, Rank> Hand { get; set; }
    }
}