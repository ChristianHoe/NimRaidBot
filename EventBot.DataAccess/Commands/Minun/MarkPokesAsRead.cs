//using EventBot.DataAccess.Database;
//using EventBot.DataAccess.Models;
//using System.Collections.Generic;
//using System.Linq;

//namespace EventBot.DataAccess.Commands.Minun
//{
//    public class MarkPokesAsReadRequest
//    {
//        public IEnumerable<PogoPokes> Pokes;
//    }

//    public interface IMarkPokesAsReadCommand : ICommand<MarkPokesAsReadRequest>
//    {
//    }
//    public sealed class MarkPokesAsReadCommand : IMarkPokesAsReadCommand
//    {
//        readonly DatabaseFactory databaseFactory;

//        public MarkPokesAsReadCommand(DatabaseFactory databaseFactory)
//        {
//            this.databaseFactory = databaseFactory;
//        }


//        public void Execute(MarkPokesAsReadRequest request)
//        {
//            using (var db = databaseFactory.CreateNew())
//            {
//                foreach(var poke in request.Pokes)
//                {
//                    var pokes = db.PogoPokes.SingleOrDefault(x => x.Id == poke.Id);
//                    if (pokes != null)
//                    {
//                        pokes.Minun = true;
//                    }
//                }
//                db.SaveChanges();
//            }
//        }
//    }
//}
