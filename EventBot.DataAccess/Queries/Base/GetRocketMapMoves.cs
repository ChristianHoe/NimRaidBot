using EventBot.Models.RocketMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EventBot.DataAccess.Queries.Base
{
    public sealed record GetRocketMapMovesRequest();

    public interface IGetRocketMapMovesQuery : IQuery<GetRocketMapMovesRequest, Dictionary<int, Move>>
    {
    }

    public sealed class GetRocketMapMoves : IGetRocketMapMovesQuery
    {
        private Dictionary<int, Move>? _cache;

        public GetRocketMapMoves()
        {
        }

        public Dictionary<int, Move> Execute(GetRocketMapMovesRequest request)
        {
            if (_cache != null)
                return _cache;

            var fileContent = File.ReadAllText(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RocketMap"), "moves.json"));

            _cache = JsonSerializer.Deserialize<Dictionary<int, Move>>(fileContent);

            if (_cache == null)
                throw new ArgumentNullException("moves.json invalid");

            return _cache;
        }
    }
}
