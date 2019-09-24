using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniShare
{
    public static class IpAddress
    {
        public static string LocalIpAddress { get; private set; }
        static IpAddress()
        {
            LocalIpAddress = Utilities.LocalIPAddress();
        }
    }
}
