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
        private ShutdownManager _shutdownManager;

        internal TCPIPListener(ILogger logger, ShutdownManager shutdownManager, TCPConfig settings)
        {
            _settings = settings;
            _logger = logger;
            _shutdownManager = shutdownManager;
        }


        internal async Task StartListener()
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
                _logger.Err("Listener.StartListener():Socket Exception: " + e.Message);                
            }
            catch(NullReferenceException e)
            {
                _logger.Err($"Listener.StartListener():Null Reference Exception: Likely the TcpSettings.");
            }
            catch(Exception e)
            {
                _logger.Err($"Listener.StartListener(): {e.Message}");
            }
            finally
            {
                if(listener != null && listener.Server.IsBound)
                {
                    listener.Stop();
                    listener.Dispose();
                    _logger.Info("Listener stopped.");
                }
                if(_shutdownManager.Status == ShutdownStatus.NotStarted)
                {
                    _logger.Warn("Task cleanup initiated via TCP/IP Listener due to error.");
                    _shutdownManager.InitiateShutdown();
                }
            }
        }

        private Task HandleClient(TcpClient client)
        {
            //
            return null;


        }

    }
}
