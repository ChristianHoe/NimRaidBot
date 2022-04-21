using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Raid
{
    public static class _Helper
    {
        public static void SetNorth(PogoRaidUser user, decimal value)
        {
            user.LatMax = value;
        }

        public static decimal? GetNorth(PogoRaidUser user)
        {
            return user.LatMax;
        }

        public static void SetEast(PogoRaidUser user, decimal value)
        {
            user.LonMax = value;
        }

        public static decimal? GetEast(PogoRaidUser user)
        {
            return user.LonMax;
        }

        public static void SetSouth(PogoRaidUser user, decimal value)
        {
            user.LatMin = value;
        }

        public static decimal? GetSouth(PogoRaidUser user)
        {
            return user.LatMin;
        }
        
        public static void SetWest(PogoRaidUser user, decimal value)
        {
            user.LonMin = value;
        }

        public static decimal? GetWest(PogoRaidUser user)
        {
            return user.LonMin;
        }
    }
}
