namespace InterruptingCards.Models
{
    public interface IPlayer
    {
        ulong Id { get; }

        string Name { get; }

        IHand Hand { get; set; }
    }
}