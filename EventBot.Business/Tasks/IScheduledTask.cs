using System.Threading;
using System.Threading.Tasks;

namespace EventBot.Business.Tasks
{
    public interface IScheduledTask
    {
        string Name { get; }

        bool RunExclusive { get; }
        int IntervallInMilliseconds { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}