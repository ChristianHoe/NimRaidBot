using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.Pokes
{
    public sealed record UpdatePokesRequest(
        IEnumerable<PogoPoke> Pokes
    );

    public interface IUpdatePokesCommand : ICommand<UpdatePokesRequest>
    {
    }
    public sealed class UpdatePokesCommand : IUpdatePokesCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UpdatePokesCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UpdatePokesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                foreach(var poke in request.Pokes)
                {
                    var current = db.PogoPokes.SingleOrDefault(x => x.Id == poke.Id);
                    if (current != null)
                    {
                        current.Iv = poke.Iv;
                        current.Cp = poke.Cp;
                        current.WeatherBoosted = poke.WeatherBoosted;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
