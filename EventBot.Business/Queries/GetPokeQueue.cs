using EventBot.DataAccess.Models;
using System.Collections.Concurrent;

namespace EventBot.Business.Queries
{
    public record GetPokeQueueRequest();

    public interface IGetPokeQueueQuery
    {
        ConcurrentQueue<PogoPoke> Execute(GetPokeQueueRequest request);
    }

    public class GetPokeQueue : IGetPokeQueueQuery
    {
        private ConcurrentQueue<PogoPoke> cache;

        private readonly DataAccess.Queries.Pokes.IGetNextNewPokeQuery getNextNewPokeQuery;

        public GetPokeQueue(
            DataAccess.Queries.Pokes.IGetNextNewPokeQuery getNextNewPokeQuery
            )
        {
            this.getNextNewPokeQuery = getNextNewPokeQuery;
        }

        public ConcurrentQueue<PogoPoke> Execute(GetPokeQueueRequest request)
        {
            if (cache != null)
                return cache;

            cache = new ConcurrentQueue<PogoPoke>();

            var waitingPokes = this.getNextNewPokeQuery.Execute(new DataAccess.Queries.Pokes.GetNextNewPokeRequest { });

            foreach (var poke in waitingPokes)
            {
                cache.Enqueue(poke);
            }

            return cache;
        }
    }
}
