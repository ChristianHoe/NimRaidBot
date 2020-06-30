using System.Collections.Concurrent;

namespace EventBot.Business.Queries
{
    public class GetThrottleRequest
    {
    }

    public class ThroughPut
    {
        public long Ticks;
        public int Count;
    }

    public interface IGetThrottleQuery
    {
        ConcurrentDictionary<long, ThroughPut> Execute(GetThrottleRequest request);
    }

    public class GetThrottle : IGetThrottleQuery
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
