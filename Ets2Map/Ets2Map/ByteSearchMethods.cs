using System.Collections.Generic;

namespace Ets2Map
{
    public static class ByteSearchMethods
    {
        public static unsafe List<int> IndexesOf(this byte[] haystack, byte[] needle)
        {
            var indexes = new List<int>();
            fixed (byte* h = haystack)
            fixed (byte* n = needle)
            {
                var i = 0;

                for (byte* hNext = h, hEnd = h + haystack.Length; hNext < hEnd; i++, hNext++)
                {
                    var found = true;
                    
                    for (byte* hInc = hNext, nInc = n, nEnd = n + needle.LongLength;
                        found && nInc < nEnd;
                        found = *nInc == *hInc, nInc++, hInc++)
                    {

                    }

                    if (found)
                    {
                        indexes.Add(i);
                    }
                }
                return indexes;
            }
        }

        public static unsafe List<int> IndexesOfUlong(this byte[] haystack, byte[] needle)
        {
            var indexes = new List<int>();
            fixed (byte* h = haystack)
            fixed (byte* n = needle)
            {
                var i = 0;

                for (byte* hNext = h, hEnd = h + haystack.Length; hNext < hEnd; i++, hNext++)
                {
                    if (*((ulong*) hNext) == *((ulong*) n))
                    {
                        indexes.Add(i);
                    }
                }

                return indexes;
            }
        }
    }
}