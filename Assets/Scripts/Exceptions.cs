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
}