using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Server.ConfigHandling;
using Server.Logging;
using Server.TaskShutdown;


namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger logger = new ConsoleLogger();
            
            try
            { 
                
                ShutdownManager shutdownManager = new ShutdownManager(logger);
                ConfigLoader configLoader = new ConfigLoader(logger, "appsettings.json");

                TCPIPListener server = new TCPIPListener(logger, shutdownManager, configLoader.TcpSettings);

                Task listenerTask = server.StartListener();
                shutdownManager.RegisterTask(listenerTask);


                //Run until user presses [Escape] key
                ConsoleKeyInfo keyPress;
                while (shutdownManager.Status == ShutdownStatus.NotStarted)
                {
                    if (Console.KeyAvailable)
                    {
                        keyPress = Console.ReadKey(true);
                        if (keyPress.Key == ConsoleKey.Escape)
                        { 
                            logger.Info("Shutdown intiated via the console.");
                            shutdownManager.InitiateShutdown();
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }


            }
            catch(Exception e)
            {
                logger.Err("Critical error encountered. Server shutting down..");   
            }
            finally
            {
                //If shutdown manager running, init shutdown
            }
            Console.WriteLine("END OF PROGRAM. PRESS ANY KEY TO EXIT.");
            Console.ReadKey();
        
        }
    }

}