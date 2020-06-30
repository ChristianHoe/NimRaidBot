using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class UpdateQuestsRequest
    {
        public IEnumerable<PogoQuests> Quests;
    }

    public interface IUpdateQuestsCommand : ICommand<UpdateQuestsRequest>
    {
    }
    public class UpdateQuests : IUpdateQuestsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UpdateQuests(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UpdateQuestsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                foreach (var quest in request.Quests)
                {
                    var current = db.PogoQuests.SingleOrDefault(x => x.StopId == quest.StopId);
                    if (current != null)
                    {
                        current.Created = quest.Created;
                        current.Reward = quest.Reward;
                        current.Task = quest.Task;
                        db.SaveChanges();
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
