using System;
using System.Linq;
using System.Threading.Tasks;
using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Raid
{
    public interface IUpdateRaidBossCommand : ICommand
    { }

    public class UpdateRaidBossCommand : Command, IUpdateRaidBossCommand
    {
        private readonly IGetActivePollByMessageId getActivePollByMessageId;
        private readonly IGetRaidByIdQuery getRaidByIdQuery;
        private readonly IGetUserByIdQuery getUserByIdQuery;
        private readonly IUpdateRaidsCommand updateRaidsCommand;

        public UpdateRaidBossCommand(
            IGetActivePollByMessageId getActivePollByMessageId,
            IGetRaidByIdQuery getRaidByIdQuery,
            IGetUserByIdQuery getUserByIdQuery,
            IUpdateRaidsCommand updateRaidsCommand
            )
        {
            this.getActivePollByMessageId = getActivePollByMessageId;
            this.getRaidByIdQuery = getRaidByIdQuery;
            this.getUserByIdQuery = getUserByIdQuery;
            this.updateRaidsCommand = updateRaidsCommand;
        }
        public override string HelpText => "Aktualisiert den Raidboss des zugehörigen Raids per Reply";
        public override string Key => "[RaidBossName]";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);
            var messageId = message.ReplyToMessage?.MessageId;

            if (messageId == null)
            {
                //await bot.SendTextMessageAsync(chatId, "Kein Reply gefunden.").ConfigureAwait(false);
                return;
            }
            
            var poll = this.getActivePollByMessageId.Execute(new GetActivePollByMessageIdRequest(MessageId: messageId.Value, ChatId: chatId));
            if (poll == null)
            {
                //await bot.SendTextMessageAsync(chatId, "Kein Poll gefunden.").ConfigureAwait(false);
                return;
            }

            var raid = this.getRaidByIdQuery.Execute(new GetRaidByIdRequest(RaidId: poll.RaidId ?? 0));
            if (raid == null)
            {
                //await bot.SendTextMessageAsync(chatId, "Kein Raid gefunden.").ConfigureAwait(false);
                return;
            }

            string[] input = message.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string poke = input.Length >= 1 ? input[0].Trim() : string.Empty;
            char? form = input.Length >= 2 ? (char?)input[1][0] : null;

            // raidboss
            var text1 = EventBot.Models.GoMap.Helper.PokeNames.Where(x => string.Compare(x.Value, poke, comparisonType: StringComparison.InvariantCultureIgnoreCase) == 0);


            if (text1.Count() == 1)
            {
                this.updateRaidsCommand.Execute(new UpdateRaidsRequest(Raids: new [] { new PogoRaids { Id = raid.Id, PokeId = text1.First().Key, PokeForm = form }}));
                
                try
                {
                    await bot.DeleteMessageAsync(chatId, base.GetMessageId(message));
                }
                catch
                {
                    // egal
                }
                return;
            }

            return;
        }
    }
}