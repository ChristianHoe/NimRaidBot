using EventBot.Models.PokeAlarm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EventBot.DataAccess.Queries.Base
{
    public class GetPokeBaseValuesRequest
    {
    }

    public interface IGetPokeBaseValuesQuery : IQuery<GetPokeBaseValuesRequest, Dictionary<int, BaseValue>>
    {
    }

    public class GetPokeBaseValues : IGetPokeBaseValuesQuery
    {
        private Dictionary<int, BaseValue> _cache;

        public GetPokeBaseValues()
        {
        }

        public Dictionary<int, BaseValue> Execute(GetPokeBaseValuesRequest request)
        {
            if (_cache != null)
                return _cache;

            var fileContent = File.ReadAllText(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PokeAlarm"), "basevalues.json"));

            return _cache = JsonConvert.DeserializeObject<Dictionary<int, BaseValue>>(fileContent);
        }
    }
}
