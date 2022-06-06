using EventBot.DataAccess.Models;
using EventBot.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Queries.PoGo
{
    public sealed class ActiveUsersQuery : IQuery<IEnumerable<DataAccess.Models.PogoUser>>
    {
        DataAccess.Queries.PoGo.IActiveUsers query;

        public ActiveUsersQuery(DataAccess.Queries.PoGo.IActiveUsers query)
        {
            this.query = query;
        }

        public IEnumerable<DataAccess.Models.PogoUser> Execute(Message message)
        {
            return this.query.Execute(new  DataAccess.Queries.PoGo.ActiveUsersRequest { });
        }
    }
}
