﻿using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.PoGo
{
    public interface IUnignoreCommand : ICommand
    { }

    public class UnignoreCommand : Command, IUnignoreCommand
    {
        readonly DataAccess.Commands.PoGo.IUnignoreCommand anCommand;

        public UnignoreCommand(
            DataAccess.Commands.PoGo.IUnignoreCommand anCommand
            )
            : base()
        {
            this.anCommand = anCommand;
        }

        public override string HelpText
        {
            get { return "Aktiviert die Benachrichtigungen. /unignore 100"; }
        }

        public override string Key
        {
            get { return "/unignore"; }
        }

        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = message.From.Id;
            int monsterId;

            if (!int.TryParse(text, out monsterId))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Unbekannte Id").ConfigureAwait(false);
                return true;
            }

            this.anCommand.Execute(new DataAccess.Commands.PoGo.UnignoreRequest { UserId = userId, MonsterId = monsterId });

            await bot.SendTextMessageAsync(message.Chat.Id, string.Format("{0} wird nun nicht mehr ignoriert", monsterId)).ConfigureAwait(false);
            return true;
        }
    }
}