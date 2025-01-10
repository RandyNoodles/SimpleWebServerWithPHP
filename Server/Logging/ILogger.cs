using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Logging
{
    internal interface ILogger
    {
        internal void Info(string message);
        internal void Err(string message);
        internal void Warn(string message);
    }
}
