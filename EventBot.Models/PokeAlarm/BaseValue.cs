using EventBot.Models.RocketMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBot.Models.PokeAlarm
{
    public class BaseValue
    { 
        public int attack;
        public int defense;
        public int stamina;
        public ElementType type1;
        public ElementType? type2;
        public bool legendary;
        public int generation;
        public double weight;
        public double height;
    }
}
