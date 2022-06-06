using System.Collections.Concurrent;

namespace EventBot.Business.Queries
{
    public sealed record GetThrottleRequest();

    public sealed record ThroughPut(
        long Ticks
    )
    {
        public int Count { get; private set;} = 1;

        public void Increment()
        {
            Count++;
        }
    }

    public interface IGetThrottleQuery
    {
        ConcurrentDictionary<long, ThroughPut> Execute(GetThrottleRequest request);
    }

    public sealed class GetThrottle : IGetThrottleQuery
    {
        private ConcurrentDictionary<long, ThroughPut> cache = new ConcurrentDictionary<long, ThroughPut>();

        public GetThrottle()
        { 
        }

        public ConcurrentDictionary<long, ThroughPut> Execute(GetThrottleRequest request)
        {
            return cache;
        }
    }
}
