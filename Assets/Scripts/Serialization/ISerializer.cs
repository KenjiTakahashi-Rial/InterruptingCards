namespace InterruptingCards.Serialization
{
    public interface ISerializer<S, D> where S : struct
    {
        S Serialize(D deserialized);

        D Deserialize(S serialized);
    }
}
