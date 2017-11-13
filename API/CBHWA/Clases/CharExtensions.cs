using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBHWA.Clases
{
    public static class CharExtensions
    {
        public static string Repeat(this char c, int count)
        {
            return new String(c, count);
        }
    }
}