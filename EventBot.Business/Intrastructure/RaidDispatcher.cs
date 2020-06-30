using EventBot.Business.Commands;
using EventBot.Business.Commands.Raid;
using System.Linq;


namespace EventBot.Business.Intrastructure
{
    public class RaidDispatcher : BaseDispatcher
    {
        public RaidDispatcher(
            IStartCommand startGroupCommand,
            // IConfigureUserCommand startPrivatCommand,
            IHelpCommand helpCommand,
            ICancelCommand cancelCommand,
            IPollAnswer raidPollAnswer,
            ISettingsCommand settingsCommand,
            // ICreateRaidCommand createRaidCommand,
            IIvCommand ivCommand,
            IPokeCommand pokeCommand,
            IGymsCommand gymsCommand,
            MemberRemoved memberRemoved,
            MembersAdded memberAdded,
            IOwnerCommand ownerCommand,
            IUpdateRaidBossCommand updateRaidBossCommand
            )
        {
            this.commands.Add(startGroupCommand);
            // this.commands.Add(startPrivatCommand);
            this.commands.Add(helpCommand);
            this.commands.Add(cancelCommand);

            this.answers.Add(raidPollAnswer);
            this.commands.Add(settingsCommand);
            // this.commands.Add(createRaidCommand);
            this.commands.Add(ivCommand);
            this.commands.Add(gymsCommand);
            this.commands.Add(pokeCommand);
            this.commands.Add(ownerCommand);
            this.commands.Add(updateRaidBossCommand);


            this.memberAdded = memberAdded;
            this.memberRemoved = memberRemoved;

            helpCommand.RegisterAllCommands(this.commands.Select(x => x.Value).ToList());
        }
    }
}
