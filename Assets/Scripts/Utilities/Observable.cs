using System;

namespace InterruptingCards.Utilities
{
    public class Observable<T>
    {
        private T _value;

        public event Action<T> OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnChanged?.Invoke(value);
            }
        }
    }
}