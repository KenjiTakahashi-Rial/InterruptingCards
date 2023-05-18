using System.Collections.Generic;
using System.Linq;

namespace InterruptingCards.Utilities
{
    public static class Helpers
    {
        public static IEnumerable<T> ToEnumerable<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}