using EventBot.Business.Interfaces;
using System.Collections.Generic;

namespace EventBot.Business.Intrastructure
{
    public static class Extensions
    {
        public static void Add(this Dictionary<string, ICommand> dict, ICommand cmd)
        {
            dict.Add(cmd.Key, cmd);
        }
    }

    public class BaseDispatcher
    {
        protected Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        protected List<IAnswer> answers = new List<IAnswer>();
        protected IMemberAdded memberAdded;
        protected IMemberRemoved memberRemoved;

        public bool CommandExists(string key)
        {
            return this.commands.ContainsKey(key);
        }

        public ICommand GetCommand(string key)
        {
            return this.commands[key];
        }

        public IAnswer GetAnswer(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            foreach (var answer in answers)
            {
                if (answer.CanExecute(callbackQuery))
                    return answer;
            }

            return null;
        }

        public IMemberAdded MemberAdded()
        {
            return memberAdded;
        }

        public IMemberRemoved MemberRemoved()
        {
            return memberRemoved;
        }
    }
}
