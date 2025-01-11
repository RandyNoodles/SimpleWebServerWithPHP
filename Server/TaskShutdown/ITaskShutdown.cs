using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TaskShutdown
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
        public ShutdownStatus Status { get;}

    }
}
