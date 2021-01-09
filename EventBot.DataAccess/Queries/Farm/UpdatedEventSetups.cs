using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record UpdatedEventSetupsRequest();

    public interface IUpdatedEventSetupsQuery : IQuery<UpdatedEventSetupsRequest, IEnumerable<EventSetups>>
    {
    }

    public class UpdatedEventSetups : IUpdatedEventSetupsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public UpdatedEventSetups(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<EventSetups> Execute(UpdatedEventSetupsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            { 
                // TODO
                var result = db.EventSetups.Where(x => x.Modified).ToList();

                foreach(var x in result)
                {
                    x.Modified = false;
                }
                db.SaveChanges();

                return result;
            }
        }
    }
}
