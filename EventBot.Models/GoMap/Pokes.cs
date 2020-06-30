namespace EventBot.Models.GoMap
{
    public class Pokes
    {
        public PokeMapResult[] pokemons;
        public ArenaMapResult[] gyms;
    }

    public class PokeMapResult
    {
        public long disappear_time;
        public long eid;
        public float latitude;
        public float longitude;
        public int pokemon_id;
        public int? iv;
    }

    public class ArenaMapResult
    {
        public int gym_id;
        public string name;
        public int team_id;
        public float latitude;
        public float longitude;
        public int gym_points;
        public int is_in_battle;
        public long time_ocuppied;
        public long ts;

        public int lvl;
        public int rpid;
        public long rs;
        public long re;

        public TrainerMapResult[] memb;
    }

    public class TrainerMapResult
    {
        public int p;
        public int cp;
        public string tn;
        public int tl;
        public long time_deploy;
    }
}
