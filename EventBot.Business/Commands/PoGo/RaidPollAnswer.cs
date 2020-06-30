using EventBot.Business.Interfaces;
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
    public interface IRaidPollAnswer : IAnswer
    { }

    public class RaidPollAnswer : IRaidPollAnswer
    {
        private readonly DataAccess.Queries.PoGo.IIsActivePoll isActivePoll;
        private readonly DataAccess.Queries.PoGo.IActivePoll activePoll;
        private readonly DataAccess.Commands.PoGo.IPollVoteUpdateCommand pollVoteUpdateCommand;
        private readonly DataAccess.Queries.PoGo.IPollVotesUsers pollVotesUsers;
        private readonly DataAccess.Commands.Raid.IAddUserCommand addUserCommand;
        private readonly DataAccess.Queries.Raid.IGetUserByIdQuery getUserByIdQuery;


        public RaidPollAnswer(
            DataAccess.Queries.PoGo.IIsActivePoll isActivePoll,
            DataAccess.Queries.PoGo.IActivePoll activePoll,
            DataAccess.Commands.PoGo.IPollVoteUpdateCommand pollVoteUpdateCommand,
            DataAccess.Queries.PoGo.IPollVotesUsers pollVotesUsers,
            DataAccess.Commands.Raid.IAddUserCommand addUserCommand,
            DataAccess.Queries.Raid.IGetUserByIdQuery getUserByIdQuery
            )
        {
            this.isActivePoll = isActivePoll;
            this.activePoll = activePoll;
            this.pollVoteUpdateCommand = pollVoteUpdateCommand;
            this.pollVotesUsers = pollVotesUsers;
            this.addUserCommand = addUserCommand;
            this.getUserByIdQuery = getUserByIdQuery;
        }

        public bool CanExecute(CallbackQuery message)
        {
            return this.isActivePoll.Execute(new DataAccess.Queries.PoGo.IsActivePollRequest { ChatId = this.GetChatId(message), MessageId = this.GetMessageId(message) });
        }

        public async Task<AnswerResult> Execute(CallbackQuery message, string text, TelegramBotClient bot)
        {
            //var messageId = this.GetMessageId(message);
            //var chatId = this.GetChatId(message);

            //this.addUserCommand.Execute(new DataAccess.Commands.Raid.AddUserRequest { UserId = message.From.Id, FirstName = message.From.FirstName });

            //var poll = this.activePoll.Execute(new DataAccess.Queries.PoGo.ActivePollRequest { ChatId = chatId, MessageId = messageId });

            //if (!int.TryParse(text, out int attendee))
            //    attendee = 1;

            //this.pollVoteUpdateCommand.Execute(new DataAccess.Commands.PoGo.PollVoteUpdateRequest { ChatId = chatId, MessageId = messageId, UserId = message.From.Id, Attendee = attendee });

            //var currentResults = this.pollVotesUsers.Execute(new DataAccess.Queries.PoGo.PollVotesRequest { ChatId = chatId, MessageId = messageId });

            //var updatedMessage = poll.TEXT + Environment.NewLine + string.Join(",", currentResults.Select(x => string.Format("{0} : {1}", x.FirstName, x.Attendee)));

            //var reply = new InlineKeyboardMarkup(new[] { new InlineKeyboardCallbackButton("0", "0"), new InlineKeyboardCallbackButton("+1", "1"), new InlineKeyboardCallbackButton("+2", "2") });
            //await bot.EditMessageTextAsync(GetChatId(message), messageId, updatedMessage, replyMarkup: reply).ConfigureAwait(false);

            return new AnswerResult();
        }

        private long GetChatId(CallbackQuery message)
        {
            return message.Message.Chat.Id;
        }

        private int GetMessageId(CallbackQuery message)
        {
            return message.Message.MessageId;
        }
    }
}
