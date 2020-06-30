//using EventBot.DataAccess.Database;
//using EventBot.DataAccess.Models;
//using System.Collections.Generic;
//using System.Linq;

//namespace EventBot.DataAccess.Commands.Pokes
//{
//    public class MarkAsReadRequest
//    {
//        public IEnumerable<PogoPokes> Pokes;
//    }

//    public interface IMarkAsReadCommand : ICommand<MarkAsReadRequest>
//    {
//    }
//    public class MarkAsReadCommand : IMarkAsReadCommand
//    {
//        readonly DatabaseFactory databaseFactory;

//        public MarkAsReadCommand(DatabaseFactory databaseFactory)
//        {
//            this.databaseFactory = databaseFactory;
//        }


//        public void Execute(MarkAsReadRequest request)
//        {
//            using (var db = databaseFactory.CreateNew())
//            {
//                var pokes = db.PogoPokes.Where(x => x.Modified == true);
//                foreach(var poke in pokes)
//                {
//                    poke.Modified = false;
//                }
//                db.SaveChanges();
//            }
//        }
//    }
//}
