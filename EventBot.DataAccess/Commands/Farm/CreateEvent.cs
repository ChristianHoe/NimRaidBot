using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public record CreateEventRequest(
        int? LocationId,
        DateTime? Start,
        DateTime? Finished,
        long? ChatId,
        int EventTypeId
    );

    public interface ICreateEventCommand : ICommand<CreateEventRequest>
    {
    }

    public class CreateEvent : ICreateEventCommand
    {
        readonly DatabaseFactory databaseFactory;

        public CreateEvent(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(CreateEventRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var ev = new IngrEvent { ChatId = request.ChatId, Start = request.Start, Finished = request.Finished, LocationId = request.LocationId, TypeId = request.EventTypeId };
                db.IngrEvents.Add(ev);
                db.SaveChanges();
            }
        }
    }
}
