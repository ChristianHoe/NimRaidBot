using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.DataAccess.ModelsEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Raid
{
    public interface IPollAnswer : IAnswer
    { }

    public class PollAnswer : IPollAnswer
    {
        private readonly DataAccess.Queries.PoGo.IIsActivePoll isActivePoll;
        private readonly DataAccess.Queries.PoGo.IActivePoll activePoll;
        private readonly DataAccess.Commands.PoGo.IPollVoteUpdateCommand pollVoteUpdateCommand;
        private readonly DataAccess.Commands.Raid.IAddUserCommand addUserCommand;
        private readonly DataAccess.Queries.Raid.IGetUserVoteQuery getUserVoteQuery;
        private readonly DataAccess.Commands.Raid.IUpdateMembershipAccessCommand updateMembershipAccessCommand;
        private readonly DataAccess.Queries.Raid.IGetUserByIdQuery getUserByIdQuery;


        public PollAnswer(
            DataAccess.Queries.PoGo.IIsActivePoll isActivePoll,
            DataAccess.Queries.PoGo.IActivePoll activePoll,
            DataAccess.Commands.PoGo.IPollVoteUpdateCommand pollVoteUpdateCommand,
            DataAccess.Commands.Raid.IAddUserCommand addUserCommand,
            DataAccess.Queries.Raid.IGetUserVoteQuery getUserVoteQuery,
            DataAccess.Commands.Raid.IUpdateMembershipAccessCommand updateMembershipAccessCommand,
            DataAccess.Queries.Raid.IGetUserByIdQuery getUserByIdQuery
            )
        {
            this.isActivePoll = isActivePoll;
            this.activePoll = activePoll;
            this.pollVoteUpdateCommand = pollVoteUpdateCommand;
            this.addUserCommand = addUserCommand;
            this.getUserVoteQuery = getUserVoteQuery;
            this.updateMembershipAccessCommand = updateMembershipAccessCommand;
            this.getUserByIdQuery = getUserByIdQuery;
        }

        public bool CanExecute(CallbackQuery message)
        {
            return this.isActivePoll.Execute(new DataAccess.Queries.PoGo.IsActivePollRequest { ChatId = this.GetChatId(message), MessageId = this.GetMessageId(message) });
        }

        public async Task<AnswerResult> Execute(CallbackQuery message, string text, TelegramBotClient bot)
        {
            var messageId = this.GetMessageId(message);
            var chatId = this.GetChatId(message);
            var userId = this.GetUserId(message);

            this.addUserCommand.Execute(new DataAccess.Commands.Raid.AddUserRequest { UserId = userId, FirstName = message.From.FirstName });

            var answer = text.Split('|');
            if (answer == null || answer.Count() != 2)
            {
                await Operator.SendMessage(bot, $"PollAnswer: Ungültige Antwort: {text}");
                return new AnswerResult();
            }

            var poll = this.activePoll.Execute(new DataAccess.Queries.PoGo.ActivePollRequest { ChatId = chatId, MessageId = messageId });
            if (poll == null)
            {
                await Operator.SendMessage(bot, $"PollAnswer: Kein gültiger Poll gefunden für Chat {chatId} Nachricht {messageId}");
                return new AnswerResult();
            }

            var voteRequest = new DataAccess.Queries.Raid.GetUserVoteRequest { ChatId = chatId, MessageId = messageId, UserId = userId, Poll = poll };

            var oldVote = this.getUserVoteQuery.Execute(voteRequest);

            var defaultAttendee = 1;
            if (oldVote == null || oldVote.Attendee == 0)
            {
                var user = this.getUserByIdQuery.Execute(new DataAccess.Queries.Raid.GetUserByIdRequest { UserId = userId });
                if (user != null)
                    defaultAttendee = user.GroupMembers ?? 1;
            }

            var attendee = oldVote == null || oldVote.Attendee == 0 ? defaultAttendee : oldVote.Attendee;
            if (answer[0] == "a")
            {
                if (!int.TryParse(answer[1], out int attendeeDifference))
                    attendeeDifference = 1;

                if (attendeeDifference < -10)
                    attendeeDifference = -10;

                if (attendeeDifference > 10)
                    attendeeDifference = 10;

                if (attendeeDifference == 0) // := cancel attendee
                    attendee = 0;
                else
                    attendee = attendee + attendeeDifference;

                if (attendee < 0)
                    attendee = 0;
            }

            var time = oldVote == null ? "" : oldVote.Time;
            if (answer[0] == "t")
            {
                var newTime = answer[1];
                if (newTime != null && newTime.Length > 5)
                    newTime = "";

                time = newTime;
            }

            var comments = oldVote?.Comment;
            if (answer[0] == "c")
            {
                if (comments == null)
                    comments = PogoUserVoteComments.None;

                if (answer[1] == "r")
                    comments ^= PogoUserVoteComments.Remote;
             }

            // no need adding a no-attend
            if (oldVote == null && attendee == 0)
                return new AnswerResult();

            // nothing changed
            if (oldVote != null && oldVote.Time == time && oldVote.Attendee == attendee && oldVote.Comment == comments)
                return new AnswerResult();

            this.pollVoteUpdateCommand.Execute(new DataAccess.Commands.PoGo.PollVoteUpdateRequest { ChatId = chatId, MessageId = messageId, UserId = message.From.Id, Poll = poll, Attendee = attendee, Time = time, Comment = (int?)comments });

            this.updateMembershipAccessCommand.Execute(new DataAccess.Commands.Raid.UpdateMembershipAccessRequest { GroupId = chatId, UserId = GetUserId(message) });

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

        private int GetUserId(CallbackQuery message)
        {
            return message.From.Id;
        }
    }
}
