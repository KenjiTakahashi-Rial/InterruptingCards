namespace InterruptingCards.Models
{
    public interface IGame<S, R> where S : Enum, IEquatable<S> where R : Enum, IEquatable<R>
    {
        IEnumerable<IPlayer<S, R>> Players { get; set; }
        void Play();
    }
}