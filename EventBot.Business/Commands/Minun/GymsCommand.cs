using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IGymsCommand : ICommand
    { }

    public class GymsCommand : Command, IGymsCommand
    {
        //private readonly IGetCurrentPokeSettings getCurrentPokeSettings;
        //private readonly TelegramProxies.NimPokeBot chatBot;

        public GymsCommand(
            //IGetCurrentPokeSettings getCurrentPokeSettings,
            //TelegramProxies.NimPokeBot chatBot
            )
        {
        //    this.getCurrentPokeSettings = getCurrentPokeSettings;
        //    this.chatBot = chatBot;
        }

        public override string Key => "/gyms";
        public override string HelpText => "De-/Aktiviert die Benachrichtigung für einzelne Gyms";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            // TODO:
            //var z = this.getCurrentPokeSettings.Execute(new GetCurrentPokeSettingsRequest { UserId = base.GetUserId(message) });


            //StringBuilder res = new StringBuilder();
            //var y = z.GroupBy(x => x.ChatId);
            //foreach (var group in y)
            //{
            //    try
            //    {
            //        // der bot muss die Gruppe kennen
            //        var chat = await chatBot.GetChatAsync(group.Key).ConfigureAwait(false);

            //        var pokesAsText = group.Select(x => x.Selected ? x.PokeId + "*" : x.PokeId.ToString());
            //        res.AppendLine($"{chat.Title}");
            //        res.AppendLine($"{string.Join(", ", pokesAsText)}");
            //    }
            //    catch (Exception ex)
            //    {
            //        await Operator.SendMessage(bot, $"/pokes für Gruppe {group.Key} fehlgeschlagen.", ex).ConfigureAwait(false);
            //    }
            //}

            //await bot.SendTextMessageAsync(GetChatId(message), res.ToString()).ConfigureAwait(false);

            return;
        }
    }
}
