using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    //What do:
    /*
     * - Listen for incoming TCP/IP
     * - Spawn new thread upon request
     * - Send to the appropriate authority to do stuff
     * - Return response from said authority
     * - Continue to listen
     */


    internal class TCPIPListener
    {
        internal TCPConfig? Settings { get; set; }

        internal event Action<string> WriteLog;
        private void Log(string s)
        {
            if(WriteLog != null)
            {
                WriteLog(s);
            }
        }


        TCPIPListener(TCPConfig? configData)
        {
            Settings = configData;
        }


        internal void StartListener(CancellationToken token)
        {

            if (Settings == null)
            {
                Log("TCP Listener aborted startup, settings null.");
                return;
            }

            TcpListener listener = null;
            try
            {
                listener = new TcpListener(Settings.IpAddress, Settings.Port);
                listener.Start();
                Log("Listener started.");


                while (!token.IsCancellationRequested)
                {

                    TcpClient client = listener.AcceptTcpClient();
                    

                    //Do all the stuff
                }
            }
        }

    }
}
