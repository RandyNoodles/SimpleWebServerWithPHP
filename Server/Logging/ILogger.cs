using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Logging
{
    public interface ILogger
    {
        public void Info(string message);
        public void Err(string message);
        public void Warn(string message);
    }
}
