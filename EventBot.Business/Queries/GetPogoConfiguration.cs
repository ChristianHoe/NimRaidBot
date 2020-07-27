using EventBot.DataAccess.Models;

namespace EventBot.Business.Queries
{
    public class GetPogoConfigurationRequest
    {
    }

    public interface IGetPogoConfigurationQuery
    {
        PogoConfigurations Execute(GetPogoConfigurationRequest request);
    }

    public class GetPogoConfiguration : IGetPogoConfigurationQuery
    {
        private PogoConfigurations? cache;

        private readonly DataAccess.Queries.PoGo.IGetPogoConfiguration getPogoConfigurationQuery;

        public GetPogoConfiguration(
            DataAccess.Queries.PoGo.IGetPogoConfiguration getPogoConfigurationQuery
            )
        {
            this.getPogoConfigurationQuery = getPogoConfigurationQuery;
        }

        public PogoConfigurations Execute(GetPogoConfigurationRequest request)
        {
            if (cache != null)
                return cache;

            return cache = this.getPogoConfigurationQuery.Execute(new DataAccess.Queries.PoGo.GetPogoConfigurationRequest());
        }
    }
}