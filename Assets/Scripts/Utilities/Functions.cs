using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InterruptingCards.Utilities
{
    public static class Functions
    {
        public static void ForEachFile
        (
            string directory,
            Action<string> action,
            bool recursive = false,
            string fileSearchPattern = "*",
            string directorySearchPattern = "*"
        )
        {
            string[] fileNames = Directory.GetFiles(directory, fileSearchPattern);

            foreach (string fileName in fileNames)
            {
                action(fileName);
            }

            if (recursive)
            {
                string[] subDirectories = Directory.GetDirectories(directory, directorySearchPattern);
                foreach (string subdirectory in subDirectories)
                {
                    ForEachFile(
                        subdirectory,
                        action,
                        recursive: true,
                        fileSearchPattern: fileSearchPattern,
                        directorySearchPattern: directorySearchPattern
                    );
                }
            }
        }

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

        public static void Shuffle<T>(IList<T> values)
        {
            var random = new Random();

            for (int i = values.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (values[i], values[j]) = (values[j], values[i]);
            }
        }
    }
}