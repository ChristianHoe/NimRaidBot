﻿using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Farm;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Queries.Farm;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Farm
{
    public interface ISetupPollCommand : ICommand
    { }

    public class SetupPollCommand : Command, ISetupPollCommand
    {
        private readonly IEventSetupQuery eventSetupQuery;
        private readonly IUpdateEventSetupCommand updateEventSetupCommand;

        public SetupPollCommand(
            IEventSetupQuery eventSetupQuery,
            IUpdateEventSetupCommand updateEventSetupCommand
            )
            : base()
        {
            this.eventSetupQuery = eventSetupQuery;
            this.updateEventSetupCommand = updateEventSetupCommand;
        }

        public override string Key => "/create";
        public override string HelpText => "Erzeugt einen neue Umfrage. /create";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);

            var msg = await bot.SendTextMessageAsync(chatId, "Initialize").ConfigureAwait(false);

            var eventSetup = new DataAccess.Models.EventSetup { ChatId = chatId, MessageId = msg.MessageId, TargetChatId = -1001372009436, Type = 2, Modified = true };
            updateEventSetupCommand.Execute(new UpdateEventSetupRequest(EventSetup: eventSetup));

            return;
        }
    }
}
