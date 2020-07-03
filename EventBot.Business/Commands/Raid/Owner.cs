using EventBot.Business.Interfaces;
using EventBot.DataAccess.Queries.Raid;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Commands.Raid
{
    public interface IOwnerCommand : ICommand
    { }

    public class OwnerCommand : Command, IOwnerCommand
    {
        private readonly IGetActivePollByMessageId getActivePollByMessageId;
        private readonly IGetRaidByIdQuery getRaidByIdQuery;
        private readonly IGetUserByIdQuery getUserByIdQuery;

        public OwnerCommand(
            IGetActivePollByMessageId getActivePollByMessageId,
            IGetRaidByIdQuery getRaidByIdQuery,
            IGetUserByIdQuery getUserByIdQuery
            )
        {
            this.getActivePollByMessageId = getActivePollByMessageId;
            this.getRaidByIdQuery = getRaidByIdQuery;
            this.getUserByIdQuery = getUserByIdQuery;
        }
        public override string HelpText => "Liest den Ersteller eines Raids aus";
        public override string Key => "/owner";

        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            var messageId = message.ReplyToMessage?.MessageId;

            if (messageId == null)
            {
                await bot.SendTextMessageAsync(chatId, "Kein Reply gefunden.").ConfigureAwait(false);
                return true;
            }
            
            var poll = this.getActivePollByMessageId.Execute(new GetActivePollByMessageIdRequest { MessageId = messageId.Value, ChatId = chatId });
            if (poll == null)
            {
                await bot.SendTextMessageAsync(chatId, "Kein Poll gefunden.").ConfigureAwait(false);
                return true;
            }

            var raid = this.getRaidByIdQuery.Execute(new GetRaidByIdRequest { RaidId = poll.RaidId ?? 0 });
            if (raid == null)
            {
                await bot.SendTextMessageAsync(chatId, "Kein Raid gefunden.").ConfigureAwait(false);
                return true;
            }

            var user = this.getUserByIdQuery.Execute(new GetUserByIdRequest { UserId = raid.Owner ?? 0 });
            if (user == null)
            {
                await bot.SendTextMessageAsync(chatId, "Nutzer nicht gefunden.").ConfigureAwait(false);
                return true;
            }

            await bot.SendTextMessageAsync(chatId, $"Ersteller: [{user.IngameName ?? user.FirstName}](tg://user?id={user.UserId})", parseMode: ParseMode.Markdown).ConfigureAwait(false);

            return true;
        }
    }
}
