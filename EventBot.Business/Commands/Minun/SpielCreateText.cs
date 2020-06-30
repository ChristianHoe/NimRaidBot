using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Queries.Base;
using EventBot.Models;
using EventBot.Models.RocketMap;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Minun
{
    public class GamePokeCreateTextRequest
    {
        public DateTime Now;

        public PogoGamePokes Game;

        public IEnumerable<PogoGamePokesAnswers> Votes;
    }

    public class GamePokeCreateTextResponse
    {
        public string Text;
        public InlineKeyboardMarkup InlineKeyboardMarkup;
        public ParseMode ParseMode;
    }

    public interface IGamePokeCreateText
    {
        GamePokeCreateTextResponse Execute(GamePokeCreateTextRequest request);
    }

    public class GamePokeCreateText : IGamePokeCreateText
    {
        private struct PokeData
        {
            public int PokeId;
            public string Name;
            public Models.PokeAlarm.BaseValue Values;
            public ElementType PokeMoveType;
        }

        private const string GLOWING_STAR = "\U0001F31F";
        private const string SHIT = "\U0001F4A9";
        private const string WHITE_CIRCLE = "\U000026AA";

        private readonly IGetPokeBaseValuesQuery getPokeBaseValuesQuery;

        public GamePokeCreateText(
            IGetPokeBaseValuesQuery getPokeBaseValuesQuery

            )
        {
            this.getPokeBaseValuesQuery = getPokeBaseValuesQuery;
        }
        
        public GamePokeCreateTextResponse Execute(GamePokeCreateTextRequest request)
        { 
            if (request.Game.Finish > request.Now)
            {
                return CurrentRun(request);
            }
            else
            {
                return Finished(request);
            }

        }

        private GamePokeCreateTextResponse CurrentRun(GamePokeCreateTextRequest request)
        {
            var pokeNames = EventBot.Models.GoMap.Helper.PokeNames;
            StringBuilder text = new StringBuilder($"{(request.Game.Finish - request.Now).Seconds} Sek.");
            text.AppendLine();
            text.AppendLine("Gegner:");
            CreatePokeWithTyping(text, request.Game.TargetPokeId, request.Game.TargetPokeMoveTyp);
            text.AppendLine("Wahl 1:");
            CreatePokeWithTyping(text, request.Game.Choice1PokeId, request.Game.Choice1PokeMoveTyp);
            text.AppendLine("Wahl 2:");
            CreatePokeWithTyping(text, request.Game.Choice2PokeId, request.Game.Choice2PokeMoveTyp);
            text.AppendLine("Wahl 3:");
            CreatePokeWithTyping(text, request.Game.Choice3PokeId, request.Game.Choice3PokeMoveTyp);
            text.AppendLine("Wahl 4:");
            CreatePokeWithTyping(text, request.Game.Choice4PokeId, request.Game.Choice4PokeMoveTyp);
            
            var keyBoardPoke1 = new[] { 
                new InlineKeyboardButton { Text = pokeNames[request.Game.Choice1PokeId], CallbackData = request.Game.Choice1PokeId.ToString() }, 
            };
            var keyBoardPoke2 = new[] { 
                new InlineKeyboardButton { Text = pokeNames[request.Game.Choice2PokeId], CallbackData = request.Game.Choice2PokeId.ToString() }, 
            };
            var keyBoardPoke3 = new[] { 
                new InlineKeyboardButton { Text = pokeNames[request.Game.Choice3PokeId], CallbackData = request.Game.Choice3PokeId.ToString() }, 
            };
            var keyBoardPoke4 = new[] { 
                new InlineKeyboardButton { Text = pokeNames[request.Game.Choice4PokeId], CallbackData = request.Game.Choice4PokeId.ToString() }, 
            };

            var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[4][] { keyBoardPoke1, keyBoardPoke2, keyBoardPoke3, keyBoardPoke4 });

            return new GamePokeCreateTextResponse { Text = text.ToString(), InlineKeyboardMarkup = inlineKeyboard,ParseMode = ParseMode.Markdown, };
        }

        private void CreatePokeWithTyping(StringBuilder text, int id, int moveType)
        {
            var values = this.getPokeBaseValuesQuery.Execute(null);
            var pokeNames = EventBot.Models.GoMap.Helper.PokeNames;
            var poke = values[id];
            var type = poke.type1;
            text.AppendLine(pokeNames[id] + " (" + Translate(poke.type1) + (poke.type2 == null ? "" : ", " +Translate(poke.type2.Value)) + ")");
            text.AppendLine($"Move:{Translate((ElementType)moveType)}");
        }

        private GamePokeCreateTextResponse Finished(GamePokeCreateTextRequest request)
        {
            var values = this.getPokeBaseValuesQuery.Execute(null);
            var targetPoke = values[request.Game.TargetPokeId];

            var pokeNames = EventBot.Models.GoMap.Helper.PokeNames;

            StringBuilder text = new StringBuilder();
            text.AppendLine("Ergebnis:");
            text.AppendLine(pokeNames[request.Game.TargetPokeId] + " (" + Translate(targetPoke.type1) + (targetPoke.type2 == null ? "" : ", " +Translate(targetPoke.type2.Value)) + ")");
            // CreateTextLine(text, request.Votes.Where(x => x.Choice == request.Game.Choice1PokeId), 
            //     request.Game.TargetPokeId, request.Game.TargetPokeMoveTyp, request.Game.Choice1PokeId, request.Game.Choice1PokeMoveTyp);
            // CreateTextLine(text, request.Votes.Where(x => x.Choice == request.Game.Choice2PokeId), 
            //     request.Game.TargetPokeId, request.Game.TargetPokeMoveTyp, request.Game.Choice2PokeId, request.Game.Choice2PokeMoveTyp);
            // CreateTextLine(text, request.Votes.Where(x => x.Choice == request.Game.Choice3PokeId), 
            //     request.Game.TargetPokeId, request.Game.TargetPokeMoveTyp, request.Game.Choice3PokeId, request.Game.Choice3PokeMoveTyp);
            // CreateTextLine(text, request.Votes.Where(x => x.Choice == request.Game.Choice4PokeId), 
            //     request.Game.TargetPokeId, request.Game.TargetPokeMoveTyp, request.Game.Choice4PokeId, request.Game.Choice4PokeMoveTyp);
         
            X(text, request);
            return new GamePokeCreateTextResponse { Text = text.ToString() };
        }

        private void X(StringBuilder text, GamePokeCreateTextRequest request)
        {
            var choice = request.Game;


            var pokeNames = EventBot.Models.GoMap.Helper.PokeNames;
            var values = this.getPokeBaseValuesQuery.Execute(null);
            var targetPoke = new PokeData { PokeMoveType = (ElementType)request.Game.TargetPokeMoveTyp, Values = values[request.Game.TargetPokeId] };
            var pokes = new PokeData[]
            {
                new PokeData { PokeId = choice.Choice1PokeId, Name = pokeNames[choice.Choice1PokeId], PokeMoveType = (ElementType)choice.Choice1PokeMoveTyp, Values = values[choice.Choice1PokeId] },
                new PokeData { PokeId = choice.Choice2PokeId, Name = pokeNames[choice.Choice2PokeId], PokeMoveType = (ElementType)choice.Choice2PokeMoveTyp, Values = values[choice.Choice2PokeId] },
                new PokeData { PokeId = choice.Choice3PokeId, Name = pokeNames[choice.Choice3PokeId], PokeMoveType = (ElementType)choice.Choice3PokeMoveTyp, Values = values[choice.Choice3PokeId] },
                new PokeData { PokeId = choice.Choice4PokeId, Name = pokeNames[choice.Choice4PokeId], PokeMoveType = (ElementType)choice.Choice4PokeMoveTyp, Values = values[choice.Choice4PokeId] },
            };

            var y = pokes.Select(x => MatchOrder(targetPoke, x));
            var grouped = y.GroupBy(x => x.Points, x => x.PokeData).OrderByDescending(x => x.Key);

            foreach (var result in grouped)
            {
                text.AppendLine(PointsAsIcon(result.Key) + " (" + result.Key.ToString("0.000") + ")" );
                foreach(var poke in result.OrderBy(x => x.Name))
                {
                    var textType = " (" + Translate(poke.Values.type1) + (poke.Values.type2 == null ? "" : ", " +Translate(poke.Values.type2.Value)) + ")";
                    text.AppendLine(" " + poke.Name + textType);
                }

                var currentPokes = result.Select(x => x.PokeId).ToArray();

                var orderedVotes = request.Votes.Where(x => currentPokes.Contains(x.Choice)).OrderBy(x => x.Created);
                foreach(var vote in orderedVotes)
                {
                    text.AppendLine("  " + vote.UserName);
                }
            }
        }

        private (double Points, PokeData PokeData) MatchOrder(PokeData targetPoke, PokeData choicePoke)
        {
            var points = CalculateDamageMultiplicator(choicePoke, targetPoke);
            points -= CalculateDamageMultiplicator(targetPoke, choicePoke);

            return (points, choicePoke);
        }

        // private void CreateTextLine(StringBuilder text, IEnumerable<PogoGamePokesAnswers> votes,
        //     int targetPokeId, int targetPokeMoveTyp, int choicePokeId, int choicePokeMoveType)
        // {
        //     var pokeNames = EventBot.Models.GoMap.Helper.PokeNames;
        //     var values = this.getPokeBaseValuesQuery.Execute(null);
        //     var targetPoke = values[targetPokeId];
        //     var choicePoke = values[choicePokeId];

        //     var points = CalculateDamageMultiplicator(choicePoke, choicePokeMoveType, targetPoke, targetPokeMoveTyp);
        //     points -= CalculateDamageMultiplicator(targetPoke, targetPokeMoveTyp, choicePoke, choicePokeMoveType);
            
        //     var textType = " (" + Translate(choicePoke.type1) + (choicePoke.type2 == null ? "" : ", " +Translate(choicePoke.type2.Value)) + ")";
        //     text.AppendLine(pokeNames[choicePokeId] + textType + " " + PointsAsIcon(points));
            
        //     text.AppendLine("Kampf-Gewichtung: " + points.ToString("0.000"));

        //     foreach(var currentVote in votes.OrderBy(x => x.Created))
        //     {
        //         text.AppendLine(currentVote.UserName);
        //     }
        // }

        private string PointsAsIcon(double points)
        {
            if (points <= -0.975)
                return SHIT + SHIT + SHIT;

            if (-0.975 < points && points <= -0.6)
                return SHIT + SHIT;

            if (-0.600 < points && points <= -0.375)
                return SHIT;

            // if (points == 0.0)
            //     return string.Empty;
            
            if (0.600 > points && points >= 0.375)
                return GLOWING_STAR;

            if (0.975 > points && points >= 0.6)
                return GLOWING_STAR + GLOWING_STAR;

            if (points >= 0.975)
                return GLOWING_STAR + GLOWING_STAR + GLOWING_STAR;  

            return WHITE_CIRCLE;
        }

        private double CalculateDamageMultiplicator(PokeData sourcePoke, PokeData targetPoke)
        {
            var damage = Types.TypeMatrix[(int)sourcePoke.PokeMoveType-1, (int)targetPoke.Values.type1-1];
            
            // vs type 2
            if (targetPoke.Values.type2.HasValue)
                damage *= Types.TypeMatrix[(int)sourcePoke.PokeMoveType-1, (int)targetPoke.Values.type2-1];

            return damage;
        }

        private string Translate(ElementType elementType)
        {
            switch(elementType)
            {
                case ElementType.Bug:
                    return "Käfer";
                case ElementType.Dark:
                    return "Dunkel";
                case ElementType.Dragon:
                    return "Drache";
                case ElementType.Electric:
                    return "Elektrisch";
                case ElementType.Fairy:
                    return "Fee";
                case ElementType.Fighting:
                    return "Kampf";
                case ElementType.Fire:
                    return "Feuer";
                case ElementType.Flying:
                    return "Flug";
                case ElementType.Ghost:
                    return "Geist";
                case ElementType.Grass:
                    return "Grass";
                case ElementType.Ground:
                    return "Boden";
                case ElementType.Ice:
                    return "Eis";
                case ElementType.Normal:
                    return "Normal";
                case ElementType.Poison:
                    return "Gift";
                case ElementType.Psychic:
                    return "Pyscho";
                case ElementType.Rock:
                    return "Gestein";
                case ElementType.Steel:
                    return "Stahl";
                case ElementType.Water:
                    return "Wasser";

                default:
                    return "Unbekannt";
            }
        }
    }
}