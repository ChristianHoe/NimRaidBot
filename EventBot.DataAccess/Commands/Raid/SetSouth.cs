﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetSouthRequest
    {
        public long ChatId;
        public decimal South;
    }

    public interface ISetSouthCommand : ICommand<SetSouthRequest>
    {
    }

    public class SetSouthCommand : ISetSouthCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetSouthCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetSouthRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    _Helper.SetSouth(result, request.South);
                    db.SaveChanges();
                }
            }
        }
    }
}