using EventBot.DataAccess.Database;
using System;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public sealed record HasDailyFarmRequest(
        long ChatId,
        DateTime Day
    );

    public interface IHasDailyFarm : IQuery<HasDailyFarmRequest, bool>
    {
    }

    public sealed class HasDailyFarm : IHasDailyFarm
    {
        readonly DatabaseFactory databaseFactory;

        public HasDailyFarm(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;        
        }

        public bool Execute(HasDailyFarmRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.IngrEvents.SingleOrDefault(x => x.ChatId == request.ChatId && x.Start == request.Day) != null;
            }
        }
    }
}
