using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Pokes
{
    public sealed record AddPokesRequest(
        IEnumerable<PogoPoke> Pokes
    );

    public interface IAddPokesCommand : ICommand<AddPokesRequest>
    {
    }

    public sealed class AddPokesCommand : IAddPokesCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddPokesCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddPokesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                db.PogoPokes.AddRange(request.Pokes);
                db.SaveChanges();
            }
        }
    }
}
