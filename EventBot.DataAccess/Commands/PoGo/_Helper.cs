using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public class _Helper
    {
        public static PogoUser CreateNewUser(telegramEntities db, long userId)
        {
            var result = InitializeNewUser(userId);
            db.PogoUser.Add(result);

            return result;
        }

        private static PogoUser InitializeNewUser(long userId)
        {
            PogoUser user = new PogoUser();
            user.UserId = userId;

            return user;
        }
    }
}
