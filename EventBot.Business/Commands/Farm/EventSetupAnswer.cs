using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Farm;
using EventBot.DataAccess.Queries.Farm;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Farm
{
    public interface IEventSetupAnswer : IAnswer
    { }

    public class EventSetupAnswer : Answer, IEventSetupAnswer
    {
        private readonly IIsActiveEventSetupQuery isActiveEventSetupQuery;
        private readonly IEventSetupQuery eventSetupQuery;
        private readonly IUpdateEventSetupCommand updateEventSetup;
        private readonly DataAccess.Commands.Raid.IAddUserCommand addUserCommand;
        private readonly IEventLocationsQuery eventLocationsQuery;
        private readonly IEventLocationByIdQuery eventLocationByIdQuery;
        private readonly ICreateEventCommand createEventCommand;


        public EventSetupAnswer(
            IIsActiveEventSetupQuery isActiveEventSetupQuery,
            IEventSetupQuery eventSetupQuery,
            IUpdateEventSetupCommand updateEventSetup,
            DataAccess.Commands.Raid.IAddUserCommand addUserCommand,
            IEventLocationsQuery eventLocationsQuery,
            IEventLocationByIdQuery eventLocationByIdQuery,
            ICreateEventCommand createEventCommand
            )
        {
            this.isActiveEventSetupQuery = isActiveEventSetupQuery;
            this.eventSetupQuery = eventSetupQuery;
            this.updateEventSetup = updateEventSetup;
            this.addUserCommand = addUserCommand;
            this.eventLocationsQuery = eventLocationsQuery;
            this.eventLocationByIdQuery = eventLocationByIdQuery;
            this.createEventCommand = createEventCommand;
        }

        public override bool CanExecute(CallbackQuery message)
        {
            return this.isActiveEventSetupQuery.Execute(new IsActiveEventSetupRequest { ChatId = this.GetChatId(message), MessageId = this.GetMessageId(message) });
        }

        public override async Task<AnswerResult> ExecuteAsync(CallbackQuery message, string text, TelegramBotClient bot)
        {
            var messageId = this.GetMessageId(message);
            var chatId = this.GetChatId(message);
            var userId = this.GetUserId(message);

            this.addUserCommand.Execute(new DataAccess.Commands.Raid.AddUserRequest(UserId: userId, FirstName: message.From.FirstName));

            var answer = text.Split('|');
            if (answer == null || answer.Count() != 2)
            {
                await Operator.SendMessage(bot, $"EventSetupAnswer: Ungültige Antwort: {text}");
                return new AnswerResult();
            }

            var oldPoll = this.eventSetupQuery.Execute(new EventSetupRequest { ChatId = chatId, MessageId = messageId });
            if (oldPoll == null)
            {
                await Operator.SendMessage(bot, $"EventSetupAnswer: Kein gültiger Poll gefunden für Chat {chatId} Nachricht {messageId}");
                return new AnswerResult();
            }


            //"<< Event", "e|-1"), new InlineKeyboardCallbackButton("Event >>", "e|1") };
            //"<< Loc", "l|-1"), new InlineKeyboardCallbackButton("Loc >>", "l|1") };
            //"-1d", "d|-1"), new InlineKeyboardCallbackButton("+1d", "d|1") };
            //"-30m", "m|-30"), new InlineKeyboardCallbackButton("+30m", "m|+30") };
            //"Erzeugen", "c|1") };

            int? type = oldPoll.Type;
            //if (answer[0] == "e")
            //{
            //    if (int.TryParse(answer[1], out int tmp))
            //    {
            //        if (!type.HasValue)

            //    }
            //}

            int? locationId = oldPoll.LocationId;
            if (answer[0] == "l")
            {
                if (locationId == null)
                {
                    locationId = 252; // Adventgemeinde
                }
                else
                {
                    if (int.TryParse(answer[1], out int next))
                    {
                       var location = this.eventLocationByIdQuery.Execute(new EventLocationByIdRequest { EventId = locationId.Value });

                        var locations = this.eventLocationsQuery.Execute(new EventLocationsRequest(Name: location.Name));

                        if (next > 0)
                        {
                            var loc = locations.Last();
                            if (loc.Id != locationId)
                            {
                                locationId = loc.Id;
                            }
                        }

                        if (next < 0)
                        {
                            var loc = locations.First();
                            if (loc.Id != locationId)
                            {
                                locationId = loc.Id;
                            }
                        }
                    }
                }
            }

            DateTime? date = oldPoll.Start;
            if (answer[0] == "d")
            {
                if (date == null)
                {
                    date = DateTime.UtcNow.Date.AddHours(19);
                }
                else
                {
                    if (int.TryParse(answer[1], out int diff))
                    {
                        if (diff < -10)
                            diff = -10;

                        if (diff > 10)
                            diff = 10;

                        var tmpdate = date.Value.AddDays(diff);

                        if (tmpdate >= DateTime.UtcNow.Date || diff > 0)
                            date = tmpdate;
                    }
                }
            }

            if (answer[0] == "m")
            {
                if (date == null)
                {
                    date = DateTime.UtcNow.Date.AddHours(19);
                }
                else
                {
                    if (int.TryParse(answer[1], out int diff))
                    {
                        if (diff < -100)
                            diff = -100;

                        if (diff > 100)
                            diff = 100;

                        var tmpdate = date.Value.AddMinutes(diff);

                        if (tmpdate >= DateTime.UtcNow.Date || diff > 0)
                            date = tmpdate;
                    }
                }
            }

            if (answer[0] == "c")
            {
                this.createEventCommand.Execute(new CreateEventRequest { ChatId = oldPoll.TargetChatId, Start = oldPoll.Start, LocationId = oldPoll.LocationId, EventTypeId = oldPoll.Type.Value });
                return new AnswerResult();
            }

            // nothing changed
            if (oldPoll != null && oldPoll.LocationId == locationId && oldPoll.Start == date && oldPoll.Type == type)
                return new AnswerResult();

            oldPoll.LocationId = locationId;
            oldPoll.Start = date;
            oldPoll.Type = type;
            oldPoll.Modified = true;

            this.updateEventSetup.Execute(new UpdateEventSetupRequest { EventSetup = oldPoll });

            return new AnswerResult();
        }
    }
}
