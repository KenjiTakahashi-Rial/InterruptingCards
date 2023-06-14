using System.Collections.Generic;
using System.Text;

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

        public static string Truncate(string s, uint byteLimit)
        {
            while (Encoding.UTF8.GetByteCount(s) > byteLimit)
            {
                s = s[..^1];
            }
            return s;
        }
    }
}