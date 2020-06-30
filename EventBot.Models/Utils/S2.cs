using Google.Common.Geometry;

namespace EventBot.Models.Utils
{
    public static class S2
    {
        public static ulong GetPokeCell(ICoordinate poke)
        {
            var latLng = S2LatLng.FromDegrees((double)poke.Latitude, (double)poke.Longitude);
            var hash = S2CellId.FromLatLng(latLng).ParentForLevel(20).Id;

            return hash;
        }
    }
}
