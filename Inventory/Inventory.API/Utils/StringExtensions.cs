using Inventory.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventory.API.Utils
{
    public static class StringExtensions
    {
        public static string HexToBinary(this string hex)
        {
            return string.Join(
                string.Empty, hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }
    }
}
