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

            var ignores = InitializeIgnore(userId);
            db.PogoIgnore.AddRange(ignores);

            return result;
        }

        private static PogoUser InitializeNewUser(long userId)
        {
            PogoUser user = new PogoUser();
            user.UserId = userId;

            return user;
        }

        private static IEnumerable<PogoIgnore> InitializeIgnore(long userId)
        {
            var ignores = new int[] { 7, 8, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 29, 30, 32, 33, 35, 37, 39, 41, 42, 43, 44, 46, 47, 48, 49, 52, 54, 60, 61, 69, 70, 72, 74, 75, 77, 79, 84, 86, 87, 90, 91, 92, 93, 96, 97, 98, 99, 104, 109, 111, 116, 117, 118, 119, 120, 122, 124, 133, 140, 161, 162, 163, 164, 165, 166, 167, 168, 177, 178, 183, 184, 187, 188, 190, 193, 194, 198, 200, 207, 215, 218, 220, 221, 223, 224 };
            return ignores.Select(x => new PogoIgnore { MonsterId = x, UserId = userId });

        }
    }
}
