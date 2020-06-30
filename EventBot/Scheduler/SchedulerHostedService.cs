using EventBot.Business.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventBot.Scheduler
{
    public class SchedulerHostedService : HostedService
    {
        // WINDOWS
        #if DEBUG
            public static int MillisecondsToWait = 5000;
#endif


        // LINUX
#if !DEBUG
            public static int MillisecondsToWait = 100;
#endif

        public event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;
            
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            var referenceTime = DateTime.UtcNow;
            
            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);
                
                await Task.Delay(TimeSpan.FromMilliseconds(MillisecondsToWait), cancellationToken);
            }

            foreach(var task in this._scheduledTasks)
            {
                task.CancellationTokenSource?.Cancel();
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;
            
            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                if (taskThatShouldRun.RunExclusive())
                {
                    taskThatShouldRun.CancellationTokenSource?.Cancel();

                    taskThatShouldRun.CancellationTokenSource = new CancellationTokenSource();
                    await Task.Delay(1000, cancellationToken);
                }

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(taskThatShouldRun.CancellationTokenSource?.Token ?? cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));
                            
                            UnobservedTaskException?.Invoke(this, args);
                            
                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            public IScheduledTask Task { get; set; }

            public DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = NextRunTime.AddMilliseconds(Task.IntervallInMilliseconds);
            }

            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }

            public bool RunExclusive()
            {
                return Task.RunExclusive;
            }
        }
    }
}