using System.Collections.Generic;

namespace EventBot.Models.RocketMap
{
    public class Pokes2
    {
        public bool lastpokemon;
        public double oNeLat;
        public double oNeLng;
        public double oSwLat;
        public double oSwLng;
        public PokeMapPoke[] pokemons;
        public Dictionary<string, PokeMapGym> gyms;
        public Dictionary<string, PokeMapStop> pokestops;
        public long timestamp;
    }

    public class PokeMapPoke
    {
        public double? catch_prob_1;
        public double? catch_prob_2;
        public double? catch_prob_3;
        public int custume;
        public int? cp; //	null
        public double? cp_multiplier; //	null
        public long disappear_time; //	1510947014000
        public string encounter_id; // MTU5ODY0OTg1MjY1MTE0ODQzMTI =
        public string form; //	null
        public int gender; //	2
        public double? height; //	null
        public int? individual_attack; //	null
        public int? individual_defense; //	null
        public int? individual_stamina; //	null
        public long last_modified;
        public decimal latitude;
        public decimal longitude;
        public int? move_1; //	null
        public int? move_2; //	null
        public int pokemon_id; //	48
        public string pokemon_name;
        public string pokemon_rarity;
        public PokemonType[] pokemon_types;
        public string rating_attack;
        public string rating_defense;
        public string spawnpoint_id; //	479e7583e1d
        public int? weather_boosted_condition;
        public double? weight; //	null
    }

    public class PokemonType
    {
        public string color; //	#a8b820
        public string type; //    Käfer
    }

    public class PokeMapGym
    {
        public decimal longitude;
        public decimal latitude;
        public string name;

        public Raid raid;
    }

    public class Raid
    {
        public long start;
        public long end;
        public int level;
        public int? pokemon_id;
        public int? move_1;
        public int? move_2;
    }

    public class PokeMapStop
    {
        //active_fort_modifier	null
        public bool enabled;
        public long last_modified;
        public long last_updated;
        public decimal latitude;
        public decimal longitude;
        //lure_expiration	null
        public string name;
        //pokemon	[]
        public string pokestop_id;
        public PokeMapQuest quest_raw;
    }

    public class PokeMapQuest
    {
        public bool is_quest; //	true
        public int? item_amount; //	200
        //public int? item_id; //	000
        public string item_type; //	Stardust
        //public int? pokemon_id; //	0
        public string quest_pokemon_name; //
        //public string quest_reward_type; //	Stardust
        //public int? quest_reward_type_raw; //	3
        //public int? quest_target; //	5
        public string quest_task; //	Lande 5 gute Würfe.
        //public string quest_type; //	Land 5 throw(s)
        //public string quest_type_raw; //	Land {0} throw(s)
        //timestam
    }

    //https://github.com/RocketMap/RocketMap/blob/develop/static/data/moves.json
    public class Move
    {
        public string name;
        public ElementType? type;
        public int? damage;
        public int? duration;
        public decimal? dps;
    }

    public enum ElementType
    {
        Bug = 7,
        Dark = 18,
        Dragon = 16,
        Electric = 13,
        Fairy = 17,
        Fighting = 2,
        Fire = 10,
        Flying = 3,
        Ghost = 8,
        Grass = 12,
        Ground = 5,
        Ice = 15,
        Normal = 1,
        Poison = 4,
        Psychic = 14,
        Rock = 6,
        Steel = 9,
        Water = 11
    }
}
