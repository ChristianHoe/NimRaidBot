using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Base
{
    public record GetPokeNamesRequest(
        string PokeName
    );

    public interface IGetPokeNamesQuery : IQuery<GetPokeNamesRequest, IEnumerable<KeyValuePair<int, string>>>
    {
    }

    public class GetPokeNames : IGetPokeNamesQuery
    {
        public GetPokeNames()
        {
        }

        public IEnumerable<KeyValuePair<int, string>> Execute(GetPokeNamesRequest request)
        {
            // if (request.PokeId.HasValue)
            // {
            //     if (EventBot.Models.GoMap.Helper.PokeNames.TryGetValue(request.PokeId.Value, out string name))
            //         return new Dictionary<int, string>() { {request.PokeId.Value, name} };

            //     return Enumerable.Empty<KeyValuePair<int, string>>();
            // }

            if (string.IsNullOrWhiteSpace(request.PokeName))
                return Enumerable.Empty<KeyValuePair<int, string>>();
                
            var pokeName = request.PokeName.Trim().ToLower();
            return EventBot.Models.GoMap.Helper.PokeNames.Where(x => x.Value.StartsWith(pokeName, StringComparison.InvariantCultureIgnoreCase)).Take(5);
        }
    }
}