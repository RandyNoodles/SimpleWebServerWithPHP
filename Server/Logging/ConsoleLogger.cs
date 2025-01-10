using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* CLASS:   Logger.cs
 */


namespace Server.Logging
{
    public class ConsoleLogger : ILogger
    {
        void ILogger.Info(string message) 
        {
            Console.WriteLine($"INFO:\t{message}");
        }
        void ILogger.Err(string message)
        {
            Console.WriteLine($"ERR:\t{message}");
        }
        void ILogger.Warn(string message)
        {
            Console.WriteLine($"WARN:\t{message}");
        }
    }
}
