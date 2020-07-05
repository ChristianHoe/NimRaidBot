using System.Threading.Tasks;
using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Minun;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun {
    public interface ISpielAnswer : IAnswer { }

    public class SpielAnswer : Answer, ISpielAnswer {
        private readonly IGameAnswerCommand gameAnswerCommand;
        
        public SpielAnswer (
            IGameAnswerCommand gameAnswerCommand
            ) 
        {
            this.gameAnswerCommand = gameAnswerCommand;
        }

        public override bool CanExecute (CallbackQuery message) {
            return true;
        }

        public override async Task<AnswerResult> ExecuteAsync (CallbackQuery message, string text, TelegramBotClient bot) {
            var messageId = this.GetMessageId (message);
            var chatId = this.GetChatId (message);
            var userId = this.GetUserId (message);

            var userName = message.From.Username ??  $"{message.From.FirstName} {message.From.LastName}";
            var pokeId = int.Parse(text);
            

            this.gameAnswerCommand.Execute(new GameAnswerRequest{UserId = userId, UserName = userName.Length <= 25 ? userName : userName.Substring(0, 25), Choice = pokeId, MessageId = messageId, ChatId = chatId });
            return new AnswerResult(EventBot.Models.GoMap.Helper.PokeNames[pokeId]);
        }
    }
}