﻿using EventBot.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.PoGo
{
    public interface IUmkreisCommand : ICommand
    { }

    public class UmkreisCommand : StatefulCommand, IUmkreisCommand
    {
        readonly DataAccess.Commands.PoGo.IIgnoreCommand anCommand;

        public UmkreisCommand(
            DataAccess.Commands.PoGo.IIgnoreCommand anCommand,
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery
            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.anCommand = anCommand;

            base.Steps.Add(0, this.Step0);
            base.Steps.Add(1, this.Step1);
        }

        public override string HelpText
        {
            get { return "Umkreis, um den die Suche ausgeführt wird. /umkreis"; }
        }

        public override string Key
        {
            get { return "/umkreis"; }
        }

        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = message.From.Id;
            await bot.SendTextMessageAsync(message.Chat.Id, "Wo bist du?", replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Send Position") { RequestLocation = true } }, true, true)).ConfigureAwait(false);
            base.NextState(message, 1);

            return false;
        }

        protected async Task<bool> Step1(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = message.From.Id;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Location)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Wo bist du?", replyMarkup: new ReplyKeyboardMarkup(new[] { new KeyboardButton("Send Position") { RequestLocation = true } }, true, true)).ConfigureAwait(false);
                return false;
            }
            var location = message.Location;
            // todo save location und area

            await bot.SendTextMessageAsync(message.Chat.Id, "Position geändert.", replyMarkup: new ReplyKeyboardRemove()).ConfigureAwait(false);
            return true;
        }
    }
}