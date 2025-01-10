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
        public void Info(string message) 
        {
            Console.WriteLine($"INFO:\t{message}");
        }
        public void Err(string message)
        {
            Console.WriteLine($"ERR:\t{message}");
        }
        public void Warn(string message)
        {
            Console.WriteLine($"WARN:\t{message}");
        }
    }
}
