namespace InterruptingCards.Models
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException() { }

        public CardNotFoundException(string message) : base(message) { }

        public CardNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class DeckEmptyException : Exception
    {
        public DeckEmptyException() { }

        public DeckEmptyException(string message) : base(message) { }

        public DeckEmptyException(string message, Exception inner) : base(message, inner) { }
    }
}