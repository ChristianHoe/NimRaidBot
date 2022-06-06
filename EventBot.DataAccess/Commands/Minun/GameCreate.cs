using System;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Minun
{
    public sealed record GameCreateRequest(
        long ChatId,
        int MessageId,
        int Difficulty,
        DateTime Finish,
        int Poke1Id,
        int Poke1Move,
        int Poke2Id,
        int Poke2Move,
        int Poke3Id,
        int Poke3Move,
        int Poke4Id,
        int Poke4Move,
        int Poke5Id,
        int Poke5Move
    );

    public interface IGameCreateCommand : ICommand<GameCreateRequest>
    {
    }

    public sealed class GameCreate : IGameCreateCommand
    {
        readonly DatabaseFactory databaseFactory;

        public GameCreate(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(GameCreateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var game = new PogoGamePoke {  
                    ChatId = request.ChatId,
                    MessageId = request.MessageId,
                    Difficulty = request.Difficulty,
                    Finish = request.Finish,
                    TargetPokeId = request.Poke1Id,
                    TargetPokeMoveTyp = request.Poke1Move,
                    Choice1PokeId = request.Poke2Id,
                    Choice1PokeMoveTyp = request.Poke2Move,
                    Choice2PokeId = request.Poke3Id,
                    Choice2PokeMoveTyp = request.Poke3Move,
                    Choice3PokeId = request.Poke4Id,
                    Choice3PokeMoveTyp = request.Poke4Move,
                    Choice4PokeId = request.Poke5Id,
                    Choice4PokeMoveTyp = request.Poke5Move
                };

                db.PogoGamePokes.Add(game);
                db.SaveChanges();
            }
        }
    }
}