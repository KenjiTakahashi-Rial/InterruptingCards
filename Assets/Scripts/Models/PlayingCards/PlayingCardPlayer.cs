namespace InterruptingCards.Models
{
    public class PlayingCardPlayer : AbstractPlayer
    {
        public PlayingCardPlayer(ulong id, string name, IHand hand = null) : base(id, name, hand) { }
    }
}