using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Raid
{
    public interface IStartCommand : ICommand
    { }

    public class StartCommand : Command, IStartCommand
    {
        private readonly IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery;
        private readonly IAddChatCommand addChatCommand;
        //private readonly IConfigureUserCommand configureUserCommand;

        public StartCommand(
            IAddChatCommand addChatCommand,
            IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery
            //IConfigureUserCommand configureUserCommand
            )
        {
            this.addChatCommand = addChatCommand;
            this.getCurrentChatSettingsQuery = getCurrentChatSettingsQuery;
            // this.configureUserCommand = configureUserCommand;
        }

        public override string HelpText
        {
            get { return "Willkommensnachricht"; }
        }

        public override string Key
        {
            get { return "/start"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            if (message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
            {
                var chatId = base.GetChatId(message);

                var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: chatId));
                if (currentSettings == null)
                {
                    this.addChatCommand.Execute(new AddChatRequest(ChatId: chatId, Name: message.Chat.Title));
                }

                await bot.SendTextMessageAsync(chatId, "Mit /settings kann der Bot konfiguriert werden.").ConfigureAwait(false);

            //if (message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
            //{
                await bot.SendTextMessageAsync(message.Chat.Id, "Bitte immer auf die Fragen des Bots per Antwort antworten, sonst werden diese dem Bot nicht zugestellt.").ConfigureAwait(false);
            }
            else
            {
                //await this.configureUserCommand.Execute(message, text, bot, 0);
            }

            return;
        }
    }
}