using Server.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Server.TaskShutdown
{
    public class ShutdownManager : ITaskShutdown
    {

        private const int DEFAULT_TIMEOUT = 10000;
        public ShutdownStatus Status { get; private set; }

        private CancellationTokenSource _cts;
        public CancellationToken GetToken()
        {
            return _cts.Token;
        }

        private ConcurrentBag<Task> _runningTasks;

        public int Timeout { get; set; }

        private ILogger _logger;

        public ShutdownManager(ILogger logger)
        {
            Timeout = DEFAULT_TIMEOUT;
            Status = ShutdownStatus.NotStarted;
            _cts = new CancellationTokenSource();
            _runningTasks = new ConcurrentBag<Task>();
            _logger = logger;
        }

        public ShutdownStatus InitiateShutdown()
        {
            if (Status != ShutdownStatus.NotStarted)
            {
                _logger.Warn($"Shutdown already in progress. New shutdown request ignored.");
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
                return Status = ShutdownStatus.Failed;
            }
            finally
            {
                _cts.Dispose();
            }


            return Status = ShutdownStatus.Success;
        }

        public bool RegisterTask(Task task)
        {
            if (Status == ShutdownStatus.NotStarted)
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
