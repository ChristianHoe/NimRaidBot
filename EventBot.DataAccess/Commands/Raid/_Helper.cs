using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Raid
{
    public static class _Helper
    {
        public static void SetNorth(PogoRaidUsers user, decimal value)
        {
            user.LatMax = value;
        }

        public static decimal? GetNorth(PogoRaidUsers user)
        {
            return user.LatMax;
        }

        public static void SetEast(PogoRaidUsers user, decimal value)
        {
            user.LonMax = value;
        }

        public static decimal? GetEast(PogoRaidUsers user)
        {
            return user.LonMax;
        }

        public static void SetSouth(PogoRaidUsers user, decimal value)
        {
            user.LatMin = value;
        }

        public static decimal? GetSouth(PogoRaidUsers user)
        {
            return user.LatMin;
        }
        
        public static void SetWest(PogoRaidUsers user, decimal value)
        {
            user.LonMin = value;
        }

        public static decimal? GetWest(PogoRaidUsers user)
        {
            return user.LonMin;
        }
    }
}
