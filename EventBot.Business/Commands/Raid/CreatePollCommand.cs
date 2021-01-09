using EventBot.DataAccess.Queries.Raid;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Raid
{
    public record CreatePollRequest(
        DataAccess.Queries.Raid.Raid Raid,
        string Text,
        InlineKeyboardMarkup InlineKeyboardMarkup,
        ParseMode ParseMode,
        int? TimeOffsetId
    );

    public interface ICreatePollCommand
    {
        Task<bool> Execute(CreatePollRequest request, long chatId, TelegramBotClient bot);
    }

    public class CreatePollCommand : ICreatePollCommand
    {
        private readonly DataAccess.Commands.PoGo.INewPollCommand newPollCommand;
        private readonly IGetActivePollByRaidId getActivePollByRaidId;

        public CreatePollCommand(
            DataAccess.Commands.PoGo.INewPollCommand newPollCommand,
            IGetActivePollByRaidId getActivePollByRaidId
            )
            : base()
        {
            this.newPollCommand = newPollCommand;
            this.getActivePollByRaidId = getActivePollByRaidId;
        }

        public async Task<bool> Execute(CreatePollRequest request, long chatId, TelegramBotClient bot)
        {
            var poll = this.getActivePollByRaidId.Execute(new GetActivePollByRaidRequest(ChatId: chatId, RaidId: request.Raid.Id));
            if (poll != null)
            {
                await Helper.Operator.SendMessage(bot, $"CreatePoll für existierend Poll/Raid aufgerufen Chat: {chatId} Raid: {request.Raid.Id}").ConfigureAwait(false);
            }
            else
            {
                var msg = await bot.SendTextMessageAsync(chatId, request.Text, parseMode: request.ParseMode, replyMarkup: request.InlineKeyboardMarkup, disableWebPagePreview: true).ConfigureAwait(false);

                this.newPollCommand.Execute(new DataAccess.Commands.PoGo.NewPollRequest(ChatId: msg.Chat.Id, MessageId: msg.MessageId, RaidId: request.Raid.Id, TimeOffsetId: request.TimeOffsetId));
            }
            return true;
        }
    }
}
