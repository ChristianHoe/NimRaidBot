﻿using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Farm
{
    public class CreatePollRequest
    {
        public int EventId;
        public string Text;
        public InlineKeyboardMarkup InlineKeyboardMarkup;
        public ParseMode ParseMode;
    }

    public interface ICreatePollCommand
    {
        Task<bool> Execute(CreatePollRequest request, long chatId, TelegramBotClient bot);
    }

    public class CreatePollCommand : ICreatePollCommand
    {
        private readonly DataAccess.Commands.PoGo.INewPollCommand newPollCommand;
        private readonly DataAccess.Queries.Farm.IGetActivePollByEventId getActivePollByEventId;

        public CreatePollCommand(
            DataAccess.Commands.PoGo.INewPollCommand newPollCommand,
            DataAccess.Queries.Farm.IGetActivePollByEventId getActivePollByRaidId
            )
            : base()
        {
            this.newPollCommand = newPollCommand;
            this.getActivePollByEventId = getActivePollByRaidId;
        }

        public async Task<bool> Execute(CreatePollRequest request, long chatId, TelegramBotClient bot)
        {
            var poll = this.getActivePollByEventId.Execute(new DataAccess.Queries.Farm.GetActivePollByEventIdRequest { ChatId = chatId, EventId = request.EventId });
            if (poll != null)
            {
                await Helper.Operator.SendMessage(bot, $"CreatePoll für existierend Poll/Event aufgerufen Chat: {chatId} Event: {request.EventId}").ConfigureAwait(false);
            }
            else
            {
                var msg = await bot.SendTextMessageAsync(chatId, request.Text, parseMode: request.ParseMode, replyMarkup: request.InlineKeyboardMarkup).ConfigureAwait(false);

                this.newPollCommand.Execute(new DataAccess.Commands.PoGo.NewPollRequest { ChatId = msg.Chat.Id, MessageId = msg.MessageId, EventId = request.EventId });
            }
            return true;
        }
    }
}
