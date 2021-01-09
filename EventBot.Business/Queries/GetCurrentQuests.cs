using System.Collections.Concurrent;
using EventBot.DataAccess.ModelsEx;

namespace EventBot.Business.Queries
{
    public record GetCurrentQuestsRequest();

    public interface IGetCurrentQuestsQuery
    {
        ConcurrentDictionary<int, Quest> Execute(GetCurrentQuestsRequest request);
    }

    public class GetCurrentQuests : IGetCurrentQuestsQuery
    {
        private ConcurrentDictionary<int, Quest> cache;

        private readonly DataAccess.Queries.Raid.IGetQuestsQuery getQuestsQuery;

        public GetCurrentQuests(
            DataAccess.Queries.Raid.IGetQuestsQuery  getQuestsQuery
            )
        {
            this.getQuestsQuery = getQuestsQuery;
        }

        public ConcurrentDictionary<int, Quest> Execute(GetCurrentQuestsRequest request)
        {
            if (cache != null)
                return cache;

            cache = new ConcurrentDictionary<int, Quest>();

            var quests = this.getQuestsQuery.Execute(new DataAccess.Queries.Raid.GetQuestsRequest());

            foreach(var quest in quests)
            {
                if (!cache.TryAdd(quest.Id, quest))
                {
                    bool pleaseCheckWhatHappend = false;
                }
            }

            return cache;
        }
    }
}