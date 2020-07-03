﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Pokes
{
    public class AddPokeNotificationRequest
    {
        public IEnumerable<PogoRelPokesChats> Notifications;
    }

    public interface IAddPokeNotificationCommand : ICommand<AddPokeNotificationRequest>
    {
    }

    public class AddPokeNotificationCommand : IAddPokeNotificationCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddPokeNotificationCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddPokeNotificationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                db.PogoRelPokesChats.AddRange(request.Notifications);
                db.SaveChanges();
            }
        }
    }
}