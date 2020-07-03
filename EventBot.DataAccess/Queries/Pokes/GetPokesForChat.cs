﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public class GetPokesForChatRequest
    {
        public long ChatId;
    }

    public interface IGetPokesForChatQuery : IQuery<GetPokesForChatRequest, IEnumerable<PogoChatPoke>>
    {
    }

    public class GetPokesForChat : IGetPokesForChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetPokesForChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoChatPoke> Execute(GetPokesForChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoChatPoke.Where(x => x.ChatId == request.ChatId && x.Show == true).ToList();
            }
        }
    }
}