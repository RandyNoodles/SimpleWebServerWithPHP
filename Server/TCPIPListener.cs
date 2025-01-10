using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.ConfigHandling;
using Server.Logging;
using Server.TaskShutdown;


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
        private TCPConfig _settings;
        private readonly ILogger _logger;
        private ITaskShutdown _shutdownManager;

        internal TCPIPListener(ILogger logger, ITaskShutdown shutdownManager, TCPConfig settings)
        {
            _settings = settings;
            _logger = logger;
            _shutdownManager = shutdownManager;
        }


        internal async void StartListener()
        {

            TcpListener listener = null;
            CancellationToken token = _shutdownManager.GetToken();


            try
            {
                listener = new TcpListener(_settings.IpAddress, _settings.Port);
                listener.Start();
                _logger.Info("Listener started.");


                while (!token.IsCancellationRequested)
                {

                    TcpClient client = await listener.AcceptTcpClientAsync(token);
                    _ = HandleClient(client);

                    //Do all the stuff
                }
            }
            catch(SocketException e)
            {
                _logger.Err("Listener:Socket Exception: " + e.Message);
            }
            finally
            {
                
                listener.Stop();
            }
        }

        private Task HandleClient(TcpClient client)
        {
            //
            return null;


        }

    }
}
