﻿using EventBot.Business.Helper;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Linq;
using EventBot.DataAccess.Commands.Raid;
using EventBot.Business.Tasks;
using System.Threading;

namespace EventBot.Business.NimRaidBot
{
    public sealed class PollRemover : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly IGetPollsToCleanUpsQuery getPollsToCleanUpsQuery;
        private readonly IGetActivePogoGroups getActiveUsers;
        private readonly IDeletePollsByIdsCommand deletePollsByIdsCommand;

        public PollRemover(
            TelegramProxies.NimRaidBot proxy,
            IGetPollsToCleanUpsQuery getPollsToCleanUpsQuery,
            IGetActivePogoGroups getActiveUsers,
            IDeletePollsByIdsCommand deletePollsByIdsCommand
            )
        {
            this.proxy = proxy;
            this.getPollsToCleanUpsQuery = getPollsToCleanUpsQuery;
            this.getActiveUsers = getActiveUsers;
            this.deletePollsByIdsCommand = deletePollsByIdsCommand;
        }

        public string Name { get { return "NimRaidBot.PollRemover"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 1 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var activeUsers = this.getActiveUsers.Execute(new GetActivePogoGroupsRequest(BotIds: new long?[] { this.proxy.BotId }));
                foreach(var user in activeUsers.Where(x => x.CleanUp.HasValue))
                {
                    DateTime cleanUpTime = DateTime.UtcNow.AddMinutes(-1 * user.CleanUp.Value);
                    var pollsToBeDeleted = this.getPollsToCleanUpsQuery.Execute(new GetPollsToCleanUpRequest(ChatId: user.ChatId, ExpiredBefore: cleanUpTime));

                    this.deletePollsByIdsCommand.Execute(new DeletePollsByIdsRequest(Ids: pollsToBeDeleted.Select(x => x.Id).ToArray()));

                    foreach (var poll in pollsToBeDeleted)
                    {
                        try
                        {
                            await proxy.DeleteMessageAsync(user.ChatId, (int)poll.MessageId).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                            await Operator.SendMessage(this.proxy, "NimRadBot.PollRemover - ", ex).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, "NimRadBot.PollRemover - ", ex).ConfigureAwait(false);
            }
        }
    }
}
