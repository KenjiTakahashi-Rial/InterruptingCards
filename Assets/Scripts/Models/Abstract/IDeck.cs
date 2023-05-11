namespace InterruptingCards.Models
{
    public interface IDeck<C> where C : ICard
    {
        int Count { get; }

        void Shuffle();

        void PlaceTop(C card);

        void PlaceBottom(C card);

        void InsertRandom(C card);

        C PeekTop();

        C DrawTop();

        C DrawBottom();

        C Remove(int cardId);
    }
}