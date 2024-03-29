﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public sealed record UpdatedEventSetupsRequest();

    public interface IUpdatedEventSetupsQuery : IQuery<UpdatedEventSetupsRequest, IEnumerable<Models.EventSetup>>
    {
    }

    public sealed class UpdatedEventSetups : IUpdatedEventSetupsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public UpdatedEventSetups(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<Models.EventSetup> Execute(UpdatedEventSetupsRequest request)
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
