// using EventBot.DataAccess.Database;
// using EventBot.DataAccess.Models;
// using System.Linq;

// namespace EventBot.DataAccess.Commands.Minun
// {
//     public class AddMinunUserRequest
//     {
//         public long UserId;
//         public string FirstName;
//     }

//     public interface IAddMinunUserCommand : ICommand<AddMinunUserRequest>
//     {
//     }

//     public class AddMinunUser : IAddMinunUserCommand
//     {
//         readonly DatabaseFactory databaseFactory;

//         public AddMinunUser(DatabaseFactory databaseFactory)
//         {
//             this.databaseFactory = databaseFactory;
//         }


//         public void Execute(AddMinunUserRequest request)
//         {
//             using (var db = databaseFactory.CreateNew())
//             {
//                 var result = db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);

//                 if (result == null)
//                 {
//                     db.PogoUser.Add(new PogoUser { UserId = request.UserId, Active = true, FirstName = request.FirstName });
//                     db.SaveChanges();
//                 }
//                 else
//                 {
//                     db.SaveChanges();
//                 }
//             }
//         }
//     }
// }
