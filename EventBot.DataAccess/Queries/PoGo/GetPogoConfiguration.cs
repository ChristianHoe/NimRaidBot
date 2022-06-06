using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.PoGo
{
    public sealed record GetPogoConfigurationRequest();

    public interface IGetPogoConfiguration : IQuery<GetPogoConfigurationRequest, PogoConfiguration>
    {
    }

    public sealed class GetPogoConfiguration : IGetPogoConfiguration
    {
        readonly DatabaseFactory databaseFactory;

        public GetPogoConfiguration(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public PogoConfiguration Execute(GetPogoConfigurationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoConfigurations.Single();
            }
        }
    }
}