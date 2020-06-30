//using EventBot.Business.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Types;

//namespace EventBot.Business.Commands.PoGo
//{
//    public interface ISetRaidLevelCommand : ICommand
//    { }

//    public class SetRaidLevelCommand : Command, ISetRaidLevelCommand
//    {
//        readonly DataAccess.Commands.PoGo.IRaidLevelCommand raidLevelCommand;

//        public SetRaidLevelCommand(
//            DataAccess.Commands.PoGo.IRaidLevelCommand raidLevelCommand
//            )
//            : base()
//        {
//            this.raidLevelCommand = raidLevelCommand;
//        }

//        public override string HelpText
//        {
//            get { return "Setzt das Minimum-Raid-Level, das erwähnt werden soll. /raid 4 bzw. /raid aus"; }
//        }

//        public override string Key
//        {
//            get { return "/raidlevel"; }
//        }

//        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
//        {
//            var userId = message.From.Id;
//            int raidLevel;

//            if (text.Equals("aus", StringComparison.InvariantCultureIgnoreCase))
//            {
//                this.raidLevelCommand.Execute(new DataAccess.Commands.PoGo.RaidLevelRequest { UserId = userId, RaidLevel = null });
//                await bot.SendTextMessageAsync(message.Chat.Id, "Raid-Benachrichtigungen sind nun aus.").ConfigureAwait(false);
//                return true;
//            }

//            if (!int.TryParse(text, out raidLevel))
//            {
//                await bot.SendTextMessageAsync(message.Chat.Id, "Unbekanntes Raid-Level").ConfigureAwait(false);
//                return true;
//            }

//            this.raidLevelCommand.Execute(new DataAccess.Commands.PoGo.RaidLevelRequest { UserId = userId, RaidLevel = raidLevel });

//            await bot.SendTextMessageAsync(message.Chat.Id, string.Format("Benachrichtigungen für Raids ab Level {0} erfolgen.", raidLevel)).ConfigureAwait(false);
//            return true;
//        }
//    }
//}
