using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public class CreateEventRequest
    {
        public int? LocationId;
        public DateTime? Start;
        public DateTime? Finished;
        public long? ChatId;
        public int EventTypeId;
    }

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
                var ev = new IngrEvents { ChatId = request.ChatId, Start = request.Start, Finished = request.Finished, LocationId = request.LocationId, TypeId = request.EventTypeId };
                db.IngrEvents.Add(ev);
                db.SaveChanges();
            }
        }
    }
}
