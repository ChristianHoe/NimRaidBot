using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBot.Business.Helper;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Tasks.NimRaidBot
{
    public class NimRaidBotInactiveUserRemover : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly IGetActivePogoGroups getActiveUsers;
        private readonly IRemoveMembershipByUserIdsCommand removeMembershipByUserIdsCommand;

        private readonly IGetUsersWithoutMinunConfigurationQuery getUsersWithoutMinunConfigurationQuery;
        private readonly IGetInactiveUsersQuery getInactiveUsersQuery;

        public NimRaidBotInactiveUserRemover(
            TelegramProxies.NimRaidBot proxy,
            IGetActivePogoGroups getActiveUsers,
            IRemoveMembershipByUserIdsCommand removeMembershipByUserIdsCommand,
            IGetUsersWithoutMinunConfigurationQuery getUsersWithoutMinunConfigurationQuery,
            IGetInactiveUsersQuery getInactiveUsersQuery            
            )
        {
            this.proxy = proxy;
            this.getActiveUsers = getActiveUsers;
            this.removeMembershipByUserIdsCommand = removeMembershipByUserIdsCommand;
            this.getUsersWithoutMinunConfigurationQuery = getUsersWithoutMinunConfigurationQuery;
            this.getInactiveUsersQuery = getInactiveUsersQuery;
        }

        public string Name { get { return "NimRaidBot.InactiveUserRemover"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 12 * 60 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var activeGroups = this.getActiveUsers.Execute(new GetActivePogoGroupsRequest(BotIds: new long[] { this.proxy.BotId }));
                foreach(var group in activeGroups.Where(x => x.KickInactive == true))
                {
                    var botPrivileges = await proxy.GetChatMemberAsync(group.ChatId, (int)this.proxy.BotId);
                    if (botPrivileges.Status != ChatMemberStatus.Creator && botPrivileges.Status != ChatMemberStatus.Administrator)
                        continue;

                    var users = this.getUsersWithoutMinunConfigurationQuery.Execute(new GetUsersWithoutMinunConfigurationRequest(GroupId: group.ChatId, Threshold: DateTime.UtcNow.AddDays(-8)));

                    foreach (var user in users)
                    {
                        await Operator.SendMessage(this.proxy, $"NimRadBot.InactiveUserRemover - No Configuration [{user.IngameName ?? user.FirstName}](tg://user?id={user.UserId})").ConfigureAwait(false);

                        try
                        {
                            await proxy.KickChatMemberAsync(group.ChatId, (int)user.UserId);
                            await proxy.UnbanChatMemberAsync(group.ChatId, (int)user.UserId);
                        }
                        catch(Exception ex)
                        {
                            await Operator.SendMessage(this.proxy, "NimRadBot.InactiveUserRemover - ", ex).ConfigureAwait(false);
                        }

                        await Task.Delay(100);
                    }

                    this.removeMembershipByUserIdsCommand.Execute(new RemoveMembershipByUserIdsRequest(GroupId: group.ChatId, UserIds: users.Select(x => x.UserId).ToArray()));
                
                    var inactiveUsers = this.getInactiveUsersQuery.Execute(new GetInactiveUsersRequest(GroupId: group.ChatId, Threshold: DateTime.UtcNow.AddMonths(-6)));
                                        
                    foreach (var user in inactiveUsers)
                    {
                        await Operator.SendMessage(this.proxy, $"NimRadBot.InactiveUserRemover - Inactive [{user.IngameName ?? user.FirstName}](tg://user?id={user.UserId})").ConfigureAwait(false);

                        try
                        {
                            await proxy.KickChatMemberAsync(group.ChatId, (int)user.UserId);
                            await proxy.UnbanChatMemberAsync(group.ChatId, (int)user.UserId);
                        }
                        catch(Exception ex)
                        {
                            await Operator.SendMessage(this.proxy, "NimRadBot.InactiveUserRemover - ", ex).ConfigureAwait(false);
                        }

                        await Task.Delay(100);
                    }

                    this.removeMembershipByUserIdsCommand.Execute(new RemoveMembershipByUserIdsRequest(GroupId: group.ChatId, UserIds: inactiveUsers.Select(x => x.UserId).ToArray()));
                
                
                
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, "NimRadBot.InactiveUserRemover - ", ex).ConfigureAwait(false);
            }
        }
    }
}