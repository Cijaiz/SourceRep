using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognizant.Octane.OrchardSTS
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length).
                Where(x => 0 == x % 2).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                ToArray();
        }
    }
}