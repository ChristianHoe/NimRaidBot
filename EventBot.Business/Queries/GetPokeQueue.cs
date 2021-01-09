using EventBot.DataAccess.Models;
using System.Collections.Concurrent;

namespace EventBot.Business.Queries
{
    public record GetPokeQueueRequest();

    public interface IGetPokeQueueQuery
    {
        ConcurrentQueue<PogoPokes> Execute(GetPokeQueueRequest request);
    }

    public class GetPokeQueue : IGetPokeQueueQuery
    {
        private ConcurrentQueue<PogoPokes> cache;

        private readonly DataAccess.Queries.Pokes.IGetNextNewPokeQuery getNextNewPokeQuery;

        public GetPokeQueue(
            DataAccess.Queries.Pokes.IGetNextNewPokeQuery getNextNewPokeQuery
            )
        {
            this.getNextNewPokeQuery = getNextNewPokeQuery;
        }

        public ConcurrentQueue<PogoPokes> Execute(GetPokeQueueRequest request)
        {
            if (cache != null)
                return cache;

            cache = new ConcurrentQueue<PogoPokes>();

            var waitingPokes = this.getNextNewPokeQuery.Execute(new DataAccess.Queries.Pokes.GetNextNewPokeRequest { });

            foreach (var poke in waitingPokes)
            {
                cache.Enqueue(poke);
            }

            return cache;
        }
    }
}
