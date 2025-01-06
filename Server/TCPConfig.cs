using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class TCPConfig
    {
        internal IPAddress IpAddress { get; set; }
        internal int Port { get; set; }

        internal TCPConfig(IPAddress ip, int port)
        {
            this.IpAddress = ip;
            this.Port = port;
        }
    }
}
