using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.DataAccess.Commands;
using EventBot.DataAccess.Commands.Minun;
using EventBot.DataAccess.Queries.Base;
using EventBot.DataAccess.Queries.Minun;
using EventBot.Models.RocketMap;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Minun
{
    public interface ISpielCommand : ICommand
    { }

    public class SpielCommand : StatefulCommand, ISpielCommand
    {
        private readonly IGetPokeBaseValuesQuery getPokeBaseValuesQuery;
        private readonly IGameCreateCommand gameCreateCommand;
        private readonly Random random = new Random();

        public SpielCommand(
            IStateUpdateCommand stateUpdateCommand, 
            IStatePushCommand statePushCommand, 
            IStatePopCommand statePopCommand, 
            StatePeakQuery statePeakQuery,

            IGetPokeBaseValuesQuery getPokeBaseValuesQuery,
            IGameCreateCommand gameCreateCommand
            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.getPokeBaseValuesQuery = getPokeBaseValuesQuery;
            this.gameCreateCommand = gameCreateCommand;

            base.Steps.Add(0, this.Step0);
            base.Steps.Add(1, this.Step1);
        }

        public override string Key => "/spiel";

        public override string HelpText => "Teste dein Wissen über die passenden Pokemon-Counter";

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);

            // FOR DEBUG
            return await this.Step1(message, text, bot);

            var msg = new StringBuilder("Schwierigkeitsgrad:");
            msg.AppendLine();
            msg.AppendLine("1 - Einfach");
            msg.AppendLine("2 - Mittel");
            msg.AppendLine("3 - Schwer");

            await bot.SendTextMessageAsync(chatId, msg.ToString()).ConfigureAwait(false);

            return StateResult.AwaitUserAt(1);
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            // if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int actionId))
            // {
            //     await bot.SendTextMessageAsync(chatId, "Es wurde keine Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
            //     return false;
            // }

            // if (actionId < 1 || 3 < actionId)
            // {
            //     await bot.SendTextMessageAsync(chatId, "Es wurde keine gültige Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
            //     return false;            
            // }
            var actionId = 1;

            var pokes = GetPoke(actionId);

            var finish = DateTime.UtcNow.AddSeconds(30);
            await bot.SendTextMessageAsync(chatId, "30 Sek. Eine Bewertung von 0.0 ist neutraler Schaden. Positive Werte bedeuten, dass Schaden gemacht wird.").ConfigureAwait(false);

            var gamemessage = await bot.SendTextMessageAsync(chatId, "... initialisiere ...").ConfigureAwait(false);
            this.gameCreateCommand.Execute(new GameCreateRequest {ChatId = chatId, MessageId = gamemessage.MessageId, Difficulty = actionId, Finish = finish,
                Poke1Id = pokes[0].Item1, Poke1Move =  pokes[0].Item2,
                Poke2Id = pokes[1].Item1, Poke2Move =  pokes[1].Item2,
                Poke3Id = pokes[2].Item1, Poke3Move =  pokes[2].Item2,
                Poke4Id = pokes[3].Item1, Poke4Move =  pokes[3].Item2,
                Poke5Id = pokes[4].Item1, Poke5Move =  pokes[4].Item2
                 });

            return StateResult.Finished;
        }

        protected (int, int)[] GetPoke(int Difficulty)
        {
            var pokes = this.getPokeBaseValuesQuery.Execute(null); 

            var (_, elementtypes) = GetEnumData<ElementType>();

            var r = random.Next(elementtypes.Count());
            var currentType = elementtypes[r];
         
            var filteredPokes = pokes.Where(x => !x.Value.type2.HasValue).ToArray();

            var selectedPoke = GetRandomPoke(filteredPokes);

            var results = new (int PokeId, int Move)[5];
            results[0].PokeId = selectedPoke;
            results[0].Move = (int)pokes[selectedPoke].type1;

            for (int i = 1; i < 5; i++)
            {
                results[i].PokeId = GetRandomPoke(filteredPokes);
                results[i].Move = (int) pokes[results[i].PokeId].type1;
            }

            return results;
        }

        protected int GetRandomPoke(IEnumerable<KeyValuePair<int, Models.PokeAlarm.BaseValue>> pokes)
        {
            var pos = random.Next(pokes.Count());
            return pokes.ElementAt(pos).Key;
        }


        /// ENUMS

        // https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Type.Enum.cs
        private (string[] enumNames, T[] enumValues) GetEnumData<T>() where T : struct, IConvertible // ab 7.3: Enum
        {
            FieldInfo[] flds = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            T[] values = new T[flds.Length];
            string[] names = new string[flds.Length];

            for (int i = 0; i < flds.Length; i++)
            {
                names[i] = flds[i].Name;
                values[i] = (T)flds[i].GetRawConstantValue();
            }

            // Insertion Sort these values in ascending order.
            // We use this O(n^2) algorithm, but it turns out that most of the time the elements are already in sorted order and
            // the common case performance will be faster than quick sorting this.
            IComparer comparer = Comparer<T>.Default;
            for (int i = 1; i < values.Length; i++)
            {
                int j = i;
                string tempStr = names[i];
                T val = values[i];
                bool exchanged = false;

                // Since the elements are sorted we only need to do one comparision, we keep the check for j inside the loop.
                while (comparer.Compare(values[j - 1], val) > 0)
                {
                    names[j] = names[j - 1];
                    values[j] = values[j - 1];
                    j--;
                    exchanged = true;
                    if (j == 0)
                        break;
                }

                if (exchanged)
                {
                    names[j] = tempStr;
                    values[j] = val;
                }
            }

            return (names, values);
        }
    }
}