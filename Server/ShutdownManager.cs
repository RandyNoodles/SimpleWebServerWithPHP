using Server.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ShutdownManager
    {
        private const int DEFAULT_TIMEOUT = 10000;
        internal enum ShutdownStatus
        {
            NotStarted,
            InProgress,
            Success,
            Failed,
            AlreadyStarted
        }
        private ShutdownStatus _shutdownStatus;
        internal ShutdownStatus Status { get { return _shutdownStatus; } }

        private CancellationTokenSource _cts;
        internal CancellationToken GetToken {get { return _cts.Token; } }

        private ConcurrentBag<Task> _runningTasks;

        internal int Timeout { get; set; }

        private ILogger _logger;

        internal ShutdownManager(ILogger logger)
        {
            Timeout = DEFAULT_TIMEOUT;
            _shutdownStatus = ShutdownStatus.NotStarted;
            _cts = new CancellationTokenSource();
            _runningTasks = new ConcurrentBag<Task>();
            _logger = logger;
        }

        internal ShutdownStatus InitiateShutdown()
        {
            if(_shutdownStatus != ShutdownStatus.NotStarted)
            {
                return ShutdownStatus.AlreadyStarted;
            }

            //Cancel token
            _cts.Cancel();

            //Make sure all tasks are wrapped up
            try
            {
                Task.WaitAll(_runningTasks.ToArray(), Timeout);
            }
            catch (Exception e)
            {
                _logger.Err($"{e.Source}::{e.Message}");
                return _shutdownStatus = ShutdownStatus.Failed;
            }
            finally
            {
                _cts.Dispose();
            }


            return _shutdownStatus = ShutdownStatus.Success;
        }

        internal bool RegisterTask(Task task)
        {
            if(_shutdownStatus == ShutdownStatus.NotStarted)
            {
                _runningTasks.Add(task);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
