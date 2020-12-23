using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.ModelsEx
{
    public class NotifyLocationExtended : NotifyLocation
    {
        public long BotId { get; set; }
    }
}