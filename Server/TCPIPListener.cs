using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Server.ConfigHandling;
using Server.HttpHandling;
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


                    Task clientTask = HandleClient(client);
                    _shutdownManager.RegisterTask(clientTask);
                    clientTask.ContinueWith(t => _shutdownManager.RemoveTask(t));

                    //What I THINK would be good:

                    /*
                    1. Get client request
                    2. Pass to 'Handle Client'
                    3. Call one of a series of functions/objects based on whether it was POST, PHP, idk...
                     */
                    
                }
            }
            catch(OperationCanceledException e)
            {
                //Do nothing, token was cancelled as intended.
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

        private async Task HandleClient(TcpClient client)
        {

            NetworkStream stream = client.GetStream();
            try
            {

                byte[] bytes = new byte[client.ReceiveBufferSize];
                StringBuilder requestBuffer = new StringBuilder();

                int bytesRead;
                while((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    requestBuffer.Append(Encoding.ASCII.GetString(bytes));
                }


                Http response = new Http();

                if(!Http.TryParseRequest(requestBuffer.ToString(), out Http parsedRequest))
                {
                    response.ResponseStartLine = new ResponseStartLine("HTTP/1.1", 400, "Bad Request",
                                                    "Cannot process request due to malformed syntax.");
                    SendResponse(stream, response);
                }
                

                Console.WriteLine(requestBuffer);






            }
            catch(Exception e)
            {
                _logger.Warn($"HandleClient() Error: {e.Message}");
            }
            finally
            {
                client.Close();
                stream.Close();
                client.Dispose();
                stream.Dispose();
                _logger.Info("Client handled.");
            }
        }

        private void SendResponse(NetworkStream stream, Http Response)
        {
            throw new NotImplementedException("Gotta make this still.");
        }
    }
}
