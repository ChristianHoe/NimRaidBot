using EventBot.Business.Helper;
using EventBot.DataAccess.Commands.Farm;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventBot.Business.Tasks.NimFarmBot
{
    public sealed class MetaCleaner : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly IClearEventMetaCommand clearEventMetaCommand;
        private readonly IClearPokeMetaCommand clearPokeMetaCommand;
        private readonly IClearPollMetaCommand clearPollMetaCommand;
        private readonly IClearRaidMetaCommand clearRaidMetaCommand;
        private readonly IClearQuestMetaCommand clearQuestMetaCommand;

        public MetaCleaner(
            TelegramProxies.NimFarmBot proxy,
            IClearEventMetaCommand clearEventMetaCommand,
            IClearPokeMetaCommand clearPokeMetaCommand,
            IClearPollMetaCommand clearPollMetaCommand,
            IClearRaidMetaCommand clearRaidMetaCommand,
            IClearQuestMetaCommand clearQuestMetaCommand

            )
        {
            this.proxy = proxy;
            this.clearEventMetaCommand = clearEventMetaCommand;
            this.clearPokeMetaCommand = clearPokeMetaCommand;
            this.clearPollMetaCommand = clearPollMetaCommand;
            this.clearRaidMetaCommand = clearRaidMetaCommand;
            this.clearQuestMetaCommand = clearQuestMetaCommand;
        }

        public string Name { get { return "NimFarmBot.MetaCleaner"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 60 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.clearPollMetaCommand.Execute(null);
                this.clearPokeMetaCommand.Execute(null);
                this.clearEventMetaCommand.Execute(null);
                this.clearRaidMetaCommand.Execute(null);
                this.clearQuestMetaCommand.Execute(null);
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, this.Name + " - ", ex).ConfigureAwait(false);
            }
        }
    }
}
