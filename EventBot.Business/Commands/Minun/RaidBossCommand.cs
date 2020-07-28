using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Minun;
using EventBot.DataAccess.Queries.Minun;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IRaidBossCommand : ICommand
    { }

    public class RaidBossCommand : StatefulCommand, IRaidBossCommand
    {
        private readonly IGetRaidBossPreferencesQuery getRaidBossPreferencesQuery;
        private readonly IPreferencedBossAddCommand preferencedBossAddCommand;
        private readonly IPreferencedBossRemoveCommand preferencedBossRemoveCommand;

        public RaidBossCommand(
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            IGetRaidBossPreferencesQuery getRaidBossPreferencesQuery,
            IPreferencedBossRemoveCommand preferencedBossRemoveCommand,
            IPreferencedBossAddCommand preferencedBossAddCommand

            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.getRaidBossPreferencesQuery = getRaidBossPreferencesQuery;
            this.preferencedBossAddCommand = preferencedBossAddCommand;
            this.preferencedBossRemoveCommand = preferencedBossRemoveCommand;


            base.Steps.Add(0, this.Step0);
            base.Steps.Add(1, this.Step1);
        }

        public override string Key => "/raidboss";
        public override string HelpText => "Konfiguriert einen Marker, für welchen Raid-Boss du teilnehmen würdest.";

        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);

            StringBuilder msg = new StringBuilder("Du hast für folgende Bosse Marker gesetzt:");
            var currentBosses = this.getRaidBossPreferencesQuery.Execute(new GetRaidBossPreferencesRequest { ChatId = chatId });
            msg.AppendLine();
            if (currentBosses.Count() == 0)
            {
                msg.AppendLine("Keine");
            }
            else
            {
                foreach (var boss in currentBosses)
                {
                    msg.AppendLine($"{boss.PokeId}");
                }
            }

            msg.AppendLine("Du kannst 'x' antworten, dann wird nichts geändert.");
            msg.Append("Poke-Id, die de- / aktiviert werden soll:");

            await bot.SendTextMessageAsync(chatId, msg.ToString()).ConfigureAwait(false);

            this.NextState(message, 1);
            return false;
        }

        protected async Task<bool> Step1(Message message, string text, TelegramBotClient bot)
        {
            if (!SkipCurrentStep(text))
            {
                var chatId = base.GetChatId(message);

                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int pokeId))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Poke-Id konnte nicht erkannt werden, bitte probiere es noch einmal.");
                    return false;
                }

                var currentBosses = this.getRaidBossPreferencesQuery.Execute(new GetRaidBossPreferencesRequest { ChatId = chatId });
                if (currentBosses.Any(x => x.PokeId == pokeId))
                {
                    this.preferencedBossRemoveCommand.Execute(new PreferencedBossRemoveRequest { ChatId = chatId, PokeId = pokeId });
                    await bot.SendTextMessageAsync(chatId, $"Boss {pokeId} entfernt.");
                }
                else
                {
                    this.preferencedBossAddCommand.Execute(new PreferencedBossAddRequest { ChatId = chatId, PokeId = pokeId });
                    await bot.SendTextMessageAsync(chatId, $"Boss {pokeId} hinzugefügt.");
                }
            }

            return true;
        }

        private bool SkipCurrentStep(string text)
        {
            // geht eigentlich nicht
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return text.Trim().ToLowerInvariant().Equals("x");
        }
    }
}
