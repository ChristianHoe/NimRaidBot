﻿using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record SetUserNameRequest(
        long UserId,
        string Name
    );

    public interface ISetUserNameCommand : ICommand<SetUserNameRequest>
    {
    }

    public class SetUserNameCommand : ISetUserNameCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetUserNameCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetUserNameRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.IngameName = request.Name.Length < 200 ? request.Name : request.Name.Substring(0, 200);
                    db.SaveChanges();
                }
            }
        }
    }
}
