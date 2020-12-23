using EventBot.Business.Commands.Minun;
using System.Linq;


namespace EventBot.Business.Intrastructure
{
    public class MinunDispatcher : BaseDispatcher
    {
        public MinunDispatcher(
            IStartCommand startCommand,
            INutzerCommand nutzerCommand,
            IAnCommand anCommand,
            IAusCommand ausCommand,
            ICreateRaidCommand createRaidCommand,
            ICreateEventCommand createEventCommand,
            IRaidBossCommand raidBossCommand,
            IPokeCommand pokeCommand,
            IModifyRaidCommand modifyRaidCommand,
            ISpielCommand spielCommand,
            IGymsCommand gymsCommand,

            Commands.IHelpCommand helpCommand,
            Commands.ICancelCommand cancelCommand,

            ISpielAnswer spielAnswer
            ) : base()
        {
            this.commands.Add(startCommand);
            this.commands.Add(nutzerCommand);
            this.commands.Add(anCommand);
            this.commands.Add(ausCommand);
            this.commands.Add(createRaidCommand);
            this.commands.Add(createEventCommand);
            this.commands.Add(raidBossCommand);
            this.commands.Add(pokeCommand);
            this.commands.Add(modifyRaidCommand);
            this.commands.Add(spielCommand);
            this.commands.Add(gymsCommand);

            this.commands.Add(helpCommand);
            this.commands.Add(cancelCommand);

            helpCommand.RegisterAllCommands(this.commands.Select(x => x.Value).ToList());

            this.answers.Add(spielAnswer);
        }
    }
}
