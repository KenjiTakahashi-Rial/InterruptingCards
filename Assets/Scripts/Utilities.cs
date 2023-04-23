using System;

namespace InterruptingCards
{
    public class Option<T>
    {
        private readonly T _value;

        public Option(T value)
        {
            _value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value => HasValue ? _value : throw new InvalidOperationException("Option does not have a value.");

        public static Option<T> None => new(default);
    }
}
