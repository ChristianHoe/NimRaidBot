using EventBot.Business.Interfaces;
using EventBot.DataAccess.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Queries
{
    public class State
    {
        public string Command;
        public int Step;
    }

    public class StatePeakQuery : IQuery<State?>
    {
        IQueryCurrentState query;

        public StatePeakQuery(IQueryCurrentState query)
        {
            this.query = query;
        }

        public State? Execute(Message message)
        {
            var result = this.query.Execute(new QueryCurrentStateRequest(ChatId: message.Chat.Id));
            if (result == null)
                return null;

            return new State { Command = result.Command, Step = result.Step };
        }
    }
}
