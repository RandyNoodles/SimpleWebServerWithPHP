using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* CLASS:   Logger.cs
 * PURPOSE: Provide methods for all other classes to log info.
 * 
 * USAGE:   Any other class can implement a logging method as a 
 */


namespace Server
{
    internal class Logger
    {
         internal void ConsoleWrite(string s)
        {
            Console.WriteLine(s);
        }
    }
}
