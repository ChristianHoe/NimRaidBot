using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBot.DataAccess.Queries.Farm
{
    public class GetEventByIdRequest
    {
        public int EventId;
    }

    public interface IGetEventById : IQuery<GetEventByIdRequest, IngrEvents>
    {
    }

    public class GetEventById : IGetEventById
    {
        readonly DatabaseFactory databaseFactory;

        public GetEventById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IngrEvents Execute(GetEventByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.IngrEvents.SingleOrDefault(x => x.Id == request.EventId);
            }
        }
    }
}
