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
    public class ObjectManagerException : Exception
    {
        public ObjectManagerException() { }

        public ObjectManagerException(string message) : base(message) { }

        public ObjectManagerException(string message, Exception inner) : base(message, inner) { }

        protected ObjectManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
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
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException() { }

        public PlayerNotFoundException(string message) : base(message) { }

        public PlayerNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected PlayerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TooManyPlayersException : Exception
    {
        public TooManyPlayersException() { }

        public TooManyPlayersException(string message) : base(message) { }

        public TooManyPlayersException(string message, Exception inner) : base(message, inner) { }

        protected TooManyPlayersException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}