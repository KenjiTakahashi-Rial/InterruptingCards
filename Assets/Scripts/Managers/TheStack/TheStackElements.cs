using System;

using Unity.Netcode;

namespace InterruptingCards.Managers.TheStack
{
    public enum TheStackElementType
    {
        Invalid,
        Loot,
        Ability,
        DiceRoll,
    }

    public struct TheStackElement : INetworkSerializable, IEquatable<TheStackElement>
    {
        private TheStackElementType _type;
        private ulong _pushedById;
        private int _value;

        public TheStackElement(TheStackElementType type, ulong pushedById, int value)
        {
            _type = type;
            _pushedById = pushedById;
            _value = value;
        }

        public readonly TheStackElementType Type => _type;

        public readonly ulong PushedById => _pushedById;

        public readonly int Value => _value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _type);
            serializer.SerializeValue(ref _pushedById);
            serializer.SerializeValue(ref _value);
        }

        public readonly bool Equals(TheStackElement other)
        {
            return Type == other.Type
                && PushedById == other.PushedById
                && Value == other.Value;
        }
    }
}
