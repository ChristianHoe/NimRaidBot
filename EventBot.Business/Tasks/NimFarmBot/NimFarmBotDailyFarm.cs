using EventBot.Business.Helper;
using EventBot.DataAccess.Commands.Farm;
using EventBot.DataAccess.Queries.Farm;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventBot.Business.Tasks.NimFarmBot
{
    public class DailyFarm : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly IGetActiveGroups activeGroups;
        private readonly IHasDailyFarm hasDailyFarm;
        private readonly ICreateEventCommand createEventCommand;

        public DailyFarm(
            TelegramProxies.NimFarmBot proxy,
            IGetActiveGroups activeGroups,
            IHasDailyFarm hasDailyFarm,
            ICreateEventCommand createEventCommand

            )
        {
            this.proxy = proxy;
            this.activeGroups = activeGroups;
            this.hasDailyFarm = hasDailyFarm;
            this.createEventCommand = createEventCommand;
        }

        public string Name { get { return "NimFarmBot.DailyFarm"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 60 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var groups = this.activeGroups.Execute(null);
                if (groups.Count() <= 0)
                    return;

                var currentFarm = DateTime.UtcNow.Date;

                foreach(var group in groups)
                {
                    if (this.hasDailyFarm.Execute(new HasDailyFarmRequest(ChatId: group.ChatId, Day: currentFarm)))
                        continue;

                    this.createEventCommand.Execute(new CreateEventRequest(ChatId: group.ChatId, Start: currentFarm, Finished: null, LocationId: null, EventTypeId: 1));
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, this.Name + " - ", ex).ConfigureAwait(false);
            }
        }
    }
}
