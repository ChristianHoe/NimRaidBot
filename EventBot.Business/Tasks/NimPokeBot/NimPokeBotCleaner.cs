using EventBot.Business.Helper;
using EventBot.Business.Tasks;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands.Pokes;
using EventBot.DataAccess.Queries.Pokes;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventBot.Business.NimPokeBot
{
    public class NimPokeBotCleaner : IScheduledTask
    {
        private readonly BaseTelegramBotClient proxy;
        private readonly IGetPokesToCleanUpQuery getPokesToCleanUpQuery;
        private readonly IGetActivePogoGroups getActiveUsers;
        private readonly IRemoveNotificationsByIdsCommand removeNotificationsByIdsCommand;

        public NimPokeBotCleaner(
            TelegramProxies.NimRaidBot proxy,
            IGetPokesToCleanUpQuery getPokesToCleanUpQuery,
            IGetActivePogoGroups getActiveUsers,
            IRemoveNotificationsByIdsCommand removeNotificationsByIdsCommand
            )
        {
            this.proxy = proxy;
            this.getPokesToCleanUpQuery = getPokesToCleanUpQuery;
            this.getActiveUsers = getActiveUsers;
            this.removeNotificationsByIdsCommand = removeNotificationsByIdsCommand;
        }

        public string Name { get { return "NimPokeBot.Cleaner"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 1 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var activeUsers = this.getActiveUsers.Execute(new GetActivePogoGroupsRequest { BotIds = new long[] { this.proxy.BotId } });
                foreach (var user in activeUsers.Where(x => x.CleanUp.HasValue))
                {
                    DateTime cleanUpTime = DateTime.UtcNow.AddMinutes(-1);
                    var pokesToBeRemoved = this.getPokesToCleanUpQuery.Execute(new GetPokesToCleanUpRequest { ChatId = user.ChatId, ExpiredBefore = cleanUpTime });

                    this.removeNotificationsByIdsCommand.Execute(new RemoveNoficationsByIdsRequest { Ids = pokesToBeRemoved.Select(x => x.Id).ToArray() });

                    foreach (var poll in pokesToBeRemoved)
                    {
                        try
                        {
                            await proxy.DeleteMessageAsync(user.ChatId, (int)poll.MessageId).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await Operator.SendMessage(this.proxy, "NimPokeBot.Cleaner - ", ex).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, "NimPokeBot.Cleaner - ", ex).ConfigureAwait(false);
            }
        }
    }
}
