using Server.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Server.ShutdownManager
{
    public class ShutdownManager : ITaskShutdown
    {

        private const int DEFAULT_TIMEOUT = 10000;
        private ShutdownStatus _status;

        public ShutdownStatus Status { get { return _status; } }

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
            _status = ShutdownStatus.NotStarted;
            _cts = new CancellationTokenSource();
            _runningTasks = new ConcurrentBag<Task>();
            _logger = logger;
        }

        public ShutdownStatus InitiateShutdown()
        {
            if (_status != ShutdownStatus.NotStarted)
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
                return _status = ShutdownStatus.Failed;
            }
            finally
            {
                _cts.Dispose();
            }


            return _status = ShutdownStatus.Success;
        }

        public bool RegisterTask(Task task)
        {
            if (_status == ShutdownStatus.NotStarted)
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
