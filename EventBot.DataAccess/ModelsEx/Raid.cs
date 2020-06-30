using System;

namespace EventBot.DataAccess.Queries.Raid
{
    public class Raid
    {
        public int Id;
        public decimal Latitude;
        public decimal Longitude;
        public int PokeId;
        public string GymName;
        public int GymId;
        public DateTime Start;
        public DateTime Until;
        public int Level;
        public long? ChatId;
        public int? MoveId;
        public long? Owner;
        public string Title;
    }
}