namespace InterruptingCards.Models
{
    public interface IPlayer<S, R> where S : Enum where R : Enum
    {
        string Name { get; }

        IHand<S, R> Hand { get; set; }
    }
}