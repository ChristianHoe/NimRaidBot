using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.PoGo
{
    public class GetPogoConfigurationRequest
    {
    }

    public interface IGetPogoConfiguration : IQuery<GetPogoConfigurationRequest, PogoConfigurations>
    {
    }

    public class GetPogoConfiguration : IGetPogoConfiguration
    {
        readonly DatabaseFactory databaseFactory;

        public GetPogoConfiguration(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public PogoConfigurations Execute(GetPogoConfigurationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoConfigurations.Single();
            }
        }
    }
}