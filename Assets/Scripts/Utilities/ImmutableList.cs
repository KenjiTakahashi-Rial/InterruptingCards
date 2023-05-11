using System.Collections;
using System.Collections.Generic;

namespace InterruptingCards.Utilities
{
    public class ImmutableList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _list;

        public ImmutableList(IEnumerable<T> source)
        {
            _list = new List<T>(source);
        }

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}