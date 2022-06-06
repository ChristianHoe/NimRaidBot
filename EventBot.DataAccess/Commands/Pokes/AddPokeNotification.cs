using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Pokes
{
    public sealed record AddPokeNotificationRequest(
        IEnumerable<PogoRelPokesChat> Notifications
    );

    public interface IAddPokeNotificationCommand : ICommand<AddPokeNotificationRequest>
    {
    }

    public sealed class AddPokeNotificationCommand : IAddPokeNotificationCommand
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
