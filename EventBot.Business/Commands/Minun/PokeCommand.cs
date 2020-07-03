using EventBot.Business.Interfaces;
using EventBot.DataAccess.Queries.Base;
using EventBot.Models;
using EventBot.Models.PokeAlarm;
using EventBot.Models.RocketMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IPokeCommand : ICommand
    { }

    public class PokeCommand : StatefulCommand, IPokeCommand
    {
        private readonly IGetPokeBaseValuesQuery getPokeBaseValuesQuery;
        private readonly IGetPokeNamesQuery getPokeNamesQuery;

        public PokeCommand(
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            IGetPokeBaseValuesQuery getPokeBaseValuesQuery,
            IGetPokeNamesQuery getPokeNamesQuery
            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.getPokeBaseValuesQuery = getPokeBaseValuesQuery;
            this.getPokeNamesQuery = getPokeNamesQuery;

            base.Steps.Add(0, this.Step0);
        }

        public override string Key => "/poke";
        public override string HelpText => "Fragt die Eigenschaften eines Pokemon ab.";

        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = GetChatId(message);

            if (!int.TryParse(text, out int pokeId))
            {
                var names = this.getPokeNamesQuery.Execute(new GetPokeNamesRequest { PokeName = text });
                if (names.Count() == 0)
                {
                    await bot.SendTextMessageAsync(chatId, "Pokemon nicht erkannt! Poke-Id:").ConfigureAwait(false);
                    return false;
                }

                if (names.Count() > 1)
                {
                    var msg = "Pokemon nicht eindeutig!" + string.Join(Environment.NewLine, names.Select(x => $"Id: {x.Key:D3} Name: {x.Value}"));
                    await bot.SendTextMessageAsync(chatId, msg).ConfigureAwait(false);
                    return false;
                }

                pokeId = names.First().Key;
            }

            var baseValues = this.getPokeBaseValuesQuery.Execute(new GetPokeBaseValuesRequest { });
            if (!baseValues.TryGetValue(pokeId, out var baseValue))
            {
                await bot.SendTextMessageAsync(chatId, "Unbekannte Poke-Id! Poke-Id:").ConfigureAwait(false);
                return false;
            }

            var min15 = CalcuateCp(15, baseValue, 10, 10, 10);
            var max15 = CalcuateCp(15, baseValue, 15, 15, 15);
            var min20 = CalcuateCp(20, baseValue, 10, 10, 10);
            var max20 = CalcuateCp(20, baseValue, 15, 15, 15);
            var min25 = CalcuateCp(25, baseValue, 10, 10, 10);
            var max25 = CalcuateCp(25, baseValue, 15, 15, 15);
            var pokeName = Models.GoMap.Helper.PokeNames[pokeId].Trim();

            var txt = new StringBuilder($"[{pokeName}](https://telegram.me/NimPokeBot?start=249)");
            txt.AppendLine();
            txt.AppendLine($"{baseValue.type1} {baseValue.type2}");
            txt.AppendLine($"L15: {min15}-{max15}");
            txt.AppendLine($"L20: {min20}-{max20}");
            txt.AppendLine($"L25: {min25}-{max25}");


            var list = new Dictionary<ElementType, int>();

            foreach (var type in (ElementType[])Enum.GetValues(typeof(ElementType)))
            {
                var x1 = Types.TypeMatrix[(int)type-1, (int)baseValue.type1-1];

                var x2 = !baseValue.type2.HasValue ? Types.NORMAL : Types.TypeMatrix[(int)type - 1, (int)baseValue.type2 - 1];

                list.Add(type, CalculateEffeciency(x1) + CalculateEffeciency(x2));
            }

            var x1_1 = list.Where(y => y.Value == 2);
            var x2_1 = list.Where(y => y.Value == 1);
            var x3_1 = list.Where(y => y.Value == -1);
            var x4_1 = list.Where(y => y.Value == -2);
            var x5_1 = list.Where(y => y.Value < -2);



            txt.AppendLine($"2x: {string.Join(", ", x1_1.Select(x => x.Key))}");
            txt.AppendLine($"1x: {string.Join(", ", x2_1.Select(x => x.Key))}");

            txt.AppendLine($"-1x: {string.Join(", ", x3_1.Select(x => x.Key))}");
            txt.AppendLine($"-2x: {string.Join(", ", x4_1.Select(x => x.Key))}");
            txt.AppendLine($">2x: {string.Join(", ", x5_1.Select(x => x.Key))}");

            await bot.SendTextMessageAsync(chatId, txt.ToString(), Telegram.Bot.Types.Enums.ParseMode.Markdown).ConfigureAwait(false);
            return true;
        }

        private int CalcuateCp(int level, BaseValue baseValue, int ivAttack, int ivDefense, int ivStamina)
        {
            var cpMultiplier = new double[] {
                0.094, 0.16639787, 0.21573247, 0.25572005, 0.29024988,
                0.3210876, 0.34921268, 0.37523559, 0.39956728, 0.42250001,
                0.44310755, 0.46279839, 0.48168495, 0.49985844, 0.51739395,
                0.53435433, 0.55079269, 0.56675452, 0.58227891, 0.59740001,
                0.61215729, 0.62656713, 0.64065295, 0.65443563, 0.667934,
                0.68116492, 0.69414365, 0.70688421, 0.71939909, 0.7317,
                0.73776948, 0.74378943, 0.74976104, 0.75568551, 0.76156384,
                0.76739717, 0.7731865, 0.77893275, 0.78463697, 0.79030001
            };

            var m = cpMultiplier[level - 1];
            var attack = (baseValue.attack + ivAttack) * m;
            var defense = (baseValue.defense + ivDefense) * m;
            var stamina = (baseValue.stamina + ivStamina) * m;

            var cp = (int)Math.Max(10, Math.Floor(Math.Sqrt(attack * attack * defense * stamina) / 10));

            return cp;
        }

        private int CalculateEffeciency(double eff)
        {
            if (eff == Types.IMMUNE)
                return -2;

            if (eff == Types.NOT_EF)
                return -1;

            if (eff == Types.VERY_E)
                return 1;

            return 0;
        }
    }
}
