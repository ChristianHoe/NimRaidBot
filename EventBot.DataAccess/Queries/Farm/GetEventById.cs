using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public sealed record GetEventByIdRequest(
        int EventId
    );

    public interface IGetEventById : IQuery<GetEventByIdRequest, IngrEvent>
    {
    }

    public sealed class GetEventById : IGetEventById
    {
        readonly DatabaseFactory databaseFactory;

        public GetEventById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IngrEvent Execute(GetEventByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.IngrEvents.SingleOrDefault(x => x.Id == request.EventId);
            }
        }
    }
}
