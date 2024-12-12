using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket_System.Service
{
    public static  class CheckAddress
    {
        public static bool IsAddress(string address)
        {
            return !string.IsNullOrEmpty(address) && address.Length > 0;
        }
    }
}