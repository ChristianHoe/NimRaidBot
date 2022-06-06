using EventBot.Models.PokeAlarm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EventBot.DataAccess.Queries.Base
{
    public sealed record GetPokeBaseValuesRequest();

    public interface IGetPokeBaseValuesQuery : IQuery<GetPokeBaseValuesRequest, Dictionary<int, BaseValue>>
    {
    }

    public sealed class GetPokeBaseValues : IGetPokeBaseValuesQuery
    {
        private Dictionary<int, BaseValue>? _cache;

        public GetPokeBaseValues()
        {
        }

        public Dictionary<int, BaseValue> Execute(GetPokeBaseValuesRequest request)
        {
            if (_cache != null)
                return _cache;

            var fileContent = File.ReadAllText(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PokeAlarm"), "basevalues.json"));

            _cache = JsonSerializer.Deserialize<Dictionary<int, BaseValue>>(fileContent);

            if (_cache == null)
                throw new ArgumentNullException("basevalues.json not valid");

            return _cache;
        }
    }
}