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


namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {

            ILogger logger = new ConsoleLogger();
            ConfigLoader configLoader = new ConfigLoader(logger);
            try
            {
                configLoader.AddJsonSource("appsettings.json");
                configLoader.LoadConfigSources();
            }
            catch(Exception e)
            {
                logger.Err($"ConfigLoader encountered a critical error. Server startup aborted.\nException:{e.Message}");
            }
            Console.ReadKey();
        }
    }

}