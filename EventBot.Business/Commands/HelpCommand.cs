﻿using EventBot.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Commands
{
    public interface IHelpCommand : ICommand
    {
        void RegisterAllCommands(IReadOnlyCollection<ICommand> commands);
    }


    public class HelpCommand : Command, IHelpCommand
    {
        private IReadOnlyCollection<ICommand>? commands;

        public void RegisterAllCommands(IReadOnlyCollection<ICommand> commands)
        {
            this.commands = commands;
        }

        public override string HelpText
        {
            get { return "Anzeige aller Befehle";  }
        }

        public override string Key
        {
            get { return "/help"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var restrictionType = this.GetChatRestrictionType(message.Chat.Type);

            var msg = this.commands == null ? string.Empty : string.Join(Environment.NewLine, this.commands.Where(x => x.ChatRestriction.HasFlag(restrictionType)).OrderBy(x => x.Key).Select(x => x.Key + Environment.NewLine + x.HelpText));

            await bot.SendTextMessageAsync(message.Chat.Id, msg).ConfigureAwait(false);
            return;
        }

        private ChatRestrictionType GetChatRestrictionType(ChatType chatType)
        {
            switch(chatType)
            {
                case ChatType.Supergroup:
                case ChatType.Group:
                    return ChatRestrictionType.Group;
                case ChatType.Private:
                    return ChatRestrictionType.Private;
                default:
                    return ChatRestrictionType.None;
            }
        }
    }
}
