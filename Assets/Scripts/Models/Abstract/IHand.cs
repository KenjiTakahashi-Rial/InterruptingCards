namespace InterruptingCards.Models
{
    public interface IHand<C> where C : ICard
    {
        int Count { get; }

        void Add(C card);

        C Remove(int cardId);

        C Get(int i);

        void Clear();
    }
}