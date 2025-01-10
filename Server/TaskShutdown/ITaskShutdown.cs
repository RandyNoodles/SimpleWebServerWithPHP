using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ShutdownManager
{
    public enum ShutdownStatus
    {
        NotStarted,
        InProgress,
        Success,
        Failed,
        AlreadyStarted
    }
    public interface ITaskShutdown
    {
        public bool RegisterTask(Task task);
        public ShutdownStatus InitiateShutdown();
        public CancellationToken GetToken();

    }
}
