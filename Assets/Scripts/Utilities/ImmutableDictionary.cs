using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

namespace InterruptingCards.Utilities
{
    public class ImmutableDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ImmutableDictionary(IDictionary<TKey, TValue> dictionary = null)
        {
            _dictionary = dictionary == null
                ? new Dictionary<TKey, TValue>()
                : new Dictionary<TKey, TValue>(dictionary);
        }

        public TValue this[TKey key] => _dictionary[key];

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        public ImmutableDictionary<NewTKey, NewTValue> ToDictionary<NewTKey, NewTValue>(
            Func<KeyValuePair<TKey, TValue>, NewTKey> keySelector,
            Func<KeyValuePair<TKey, TValue>, NewTValue> valueSelector
        )
        {
            try
            {
                return new ImmutableDictionary<NewTKey, NewTValue>(_dictionary.ToDictionary(keySelector, valueSelector));
            }
            catch
            {
                foreach ((var k, var v) in _dictionary)
                {
                    Debug.LogError($"{{ {k}: {v} }}");
                }

                throw;
            }
        }
    }
}
