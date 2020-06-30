using EventBot.Models.RocketMap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EventBot.DataAccess.Queries.Base
{
    public class GetRocketMapMovesRequest
    {
    }

    public interface IGetRocketMapMovesQuery : IQuery<GetRocketMapMovesRequest, Dictionary<int, Move>>
    {
    }

    public class GetRocketMapMoves : IGetRocketMapMovesQuery
    {
        private Dictionary<int, Move> _cache;

        public GetRocketMapMoves()
        {
        }

        public Dictionary<int, Move> Execute(GetRocketMapMovesRequest request)
        {
            if (_cache != null)
                return _cache;

            var fileContent = File.ReadAllText(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RocketMap"), "moves.json"));

            _cache = JsonConvert.DeserializeObject<Dictionary<int, Move>>(fileContent);
            return _cache;
        }
    }
}
