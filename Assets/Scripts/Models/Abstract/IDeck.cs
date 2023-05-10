namespace InterruptingCards.Models
{
    public interface IDeck
    {
        int Count { get; }

        void Shuffle();

        void PlaceTop(ICard card);

        void PlaceBottom(ICard card);

        void InsertRandom(ICard card);

        ICard PeekTop();

        ICard DrawTop();

        ICard DrawBottom();

        ICard Remove(int cardId);
    }
}