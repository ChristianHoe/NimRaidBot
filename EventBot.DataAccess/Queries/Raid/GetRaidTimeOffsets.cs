using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetRaidTimeOffsetsRequest(
        int? OffsetId
    );

    public interface IGetRaidTimeOffsetsQuery : IQuery<GetRaidTimeOffsetsRequest, IEnumerable<int>>
    {
    }

    public class GetRaidTimeOffsets : IGetRaidTimeOffsetsQuery
    {
        readonly DatabaseFactory databaseFactory;
        private ILookup<int, int>? _cache;


        public GetRaidTimeOffsets(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public IEnumerable<int> Execute(GetRaidTimeOffsetsRequest request)
        {
            if (_cache != null)
                return _cache[request.OffsetId ?? 1];

            using (var db = databaseFactory.CreateNew())
            {
                _cache = db.PogoRaidTimeOffsets.OrderBy(x => x.Order).ToLookup(x => x.SettingId, y => y.OffsetInMinutes );
            }

            return _cache[request.OffsetId ?? 1];
        }
    }
}