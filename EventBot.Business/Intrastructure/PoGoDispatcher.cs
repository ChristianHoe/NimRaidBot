using EventBot.Business.Commands.PoGo;
using EventBot.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBot.Business.Intrastructure
{
    public class PoGoDispatcher : BaseDispatcher
    {
        public PoGoDispatcher(
            IAnCommand anCommand,
            IAusCommand ausCommand,
            IIgnoreCommand ignoreCommand,
            IUnignoreCommand unignoreCommand,
            //ISetRaidLevelCommand setRaidLevelCommand,
            IUmkreisCommand umkreisCommand,
            IPollCommand raidPollCommand,
            IRaidPollAnswer raidPollAnswer
            )
        {
            this.commands.Add(anCommand);
            this.commands.Add(ausCommand);

            this.commands.Add(ignoreCommand);
            this.commands.Add(unignoreCommand);

            //this.commands.Add(setRaidLevelCommand);
            this.commands.Add(umkreisCommand);

            this.commands.Add(raidPollCommand);

            this.answers.Add(raidPollAnswer);

            //helpCommand.RegisterAllCommands(this.commands.Select(x => x.Value).ToList());
        }
    }
}
