using System;
using System.Runtime.Serialization;

namespace InterruptingCards
{
    [Serializable]
    public class NetworkManagerException : Exception
    {
        public NetworkManagerException() { }

        public NetworkManagerException(string message) : base(message) { }

        public NetworkManagerException(string message, Exception inner) : base(message, inner) { }

        protected NetworkManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException() { }

        public CardNotFoundException(string message) : base(message) { }

        public CardNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected CardNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class CardCollectionEmptyException : Exception
    {
        public CardCollectionEmptyException() { }

        public CardCollectionEmptyException(string message) : base(message) { }

        public CardCollectionEmptyException(string message, Exception inner) : base(message, inner) { }

        protected CardCollectionEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TooManyCardsException : Exception
    {
        public TooManyCardsException() { }

        public TooManyCardsException(string message) : base(message) { }

        public TooManyCardsException(string message, Exception inner) : base(message, inner) { }

        protected TooManyCardsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TooManyPlayersException : Exception
    {
        public TooManyPlayersException() { }

        public TooManyPlayersException(string message) : base(message) { }

        public TooManyPlayersException(string message, Exception inner) : base(message, inner) { }

        protected TooManyPlayersException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class CardIndexOutOfRangeException : Exception
    {
        public CardIndexOutOfRangeException() { }

        public CardIndexOutOfRangeException(string message) : base(message) { }

        public CardIndexOutOfRangeException(string message, Exception inner) : base(message, inner) { }

        protected CardIndexOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}