using EventBot.DataAccess.Models;

namespace EventBot.Business.Queries
{
    public sealed record GetPogoConfigurationRequest();

    public interface IGetPogoConfigurationQuery
    {
        PogoConfiguration Execute(GetPogoConfigurationRequest request);
    }

    public sealed class GetPogoConfiguration : IGetPogoConfigurationQuery
    {
        private PogoConfiguration? cache;

        private readonly DataAccess.Queries.PoGo.IGetPogoConfiguration getPogoConfigurationQuery;

        public GetPogoConfiguration(
            DataAccess.Queries.PoGo.IGetPogoConfiguration getPogoConfigurationQuery
            )
        {
            this.getPogoConfigurationQuery = getPogoConfigurationQuery;
        }

        public PogoConfiguration Execute(GetPogoConfigurationRequest request)
        {
            if (cache != null)
                return cache;

            return cache = this.getPogoConfigurationQuery.Execute(new DataAccess.Queries.PoGo.GetPogoConfigurationRequest());
        }
    }
}