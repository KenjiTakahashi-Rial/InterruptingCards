using System;

namespace InterruptingCards
{
    public class NetworkManagerException : Exception
    {
        public NetworkManagerException() { }

        public NetworkManagerException(string message) : base(message) { }

        public NetworkManagerException(string message, Exception inner) : base(message, inner) { }
    }

    public class ObjectManagerException : Exception
    {
        public ObjectManagerException() { }

        public ObjectManagerException(string message) : base(message) { }

        public ObjectManagerException(string message, Exception inner) : base(message, inner) { }
    }
}