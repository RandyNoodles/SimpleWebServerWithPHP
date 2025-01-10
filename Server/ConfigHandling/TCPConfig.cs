using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.ConfigHandling
{
    public class TCPConfig
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }

        public TCPConfig(IPAddress ip, int port)
        {
            IpAddress = ip;
            Port = port;
        }
    }
}
