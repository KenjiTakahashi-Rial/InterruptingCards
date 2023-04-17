namespace InterruptingCards.Models
{
    public interface ICard<S, R> where S : Enum where R : Enum
    {
        S Suit { get; }
        R Rank { get; }
    }
}