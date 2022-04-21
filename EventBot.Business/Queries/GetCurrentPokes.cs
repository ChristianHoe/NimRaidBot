using EventBot.DataAccess.Models;
using System.Collections.Concurrent;


namespace EventBot.Business.Queries
{
    public record GetCurrentPokesRequest(
        int MapId
    );

    public interface IGetCurrentPokesQuery
    {
        ConcurrentDictionary<ulong, PogoPoke> Execute(GetCurrentPokesRequest request);
    }

    public class GetCurrentPokes : IGetCurrentPokesQuery
    {
        private ConcurrentDictionary<int, ConcurrentDictionary<ulong, PogoPoke>> cache = new ConcurrentDictionary<int, ConcurrentDictionary<ulong, PogoPoke>>();

        private readonly DataAccess.Queries.Pokes.IGetCurrentPokesQuery getCurrentPokesQuery;

        public GetCurrentPokes(
            DataAccess.Queries.Pokes.IGetCurrentPokesQuery getCurrentPokesQuery
            )
        {
            this.getCurrentPokesQuery = getCurrentPokesQuery;
        }

        public ConcurrentDictionary<ulong, PogoPoke> Execute(GetCurrentPokesRequest request)
        {
            if (cache.TryGetValue(request.MapId, out var result))
                return result;


            var pokelist = new ConcurrentDictionary<ulong, PogoPoke>();
            var pokes = this.getCurrentPokesQuery.Execute(new DataAccess.Queries.Pokes.GetCurrentPokesRequest(MapId: request.MapId));

            foreach(var poke in pokes)
            {
                var hash = Models.Utils.S2.GetPokeCell(poke);

                if (!pokelist.TryAdd(hash, poke))
                {
                    if (!pokelist.TryGetValue(hash, out var oldPoke))
                    {
                        bool pleaseCheckWhatHappend = false;
                    }

                    if (poke.Finished > oldPoke.Finished)
                    {
                        if (!pokelist.TryUpdate(hash, poke, oldPoke))
                        {
                            bool pleaseCheckWhatHappend = false;
                        }
                    }
                }
            }

            if (!cache.TryAdd(request.MapId, pokelist))
            {
                if (cache.TryGetValue(request.MapId, out var result2))
                    return result2;

                bool pleaseCheckWhatHappend = false;
            }

            return pokelist;
        }
    }
}
