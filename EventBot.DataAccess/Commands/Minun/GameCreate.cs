using System;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Minun
{
    public class GameCreateRequest
    {
        public long ChatId;
        public int MessageId;

        public int Difficulty;

        public DateTime Finish;

        public int Poke1Id;
        public int Poke1Move;
        public int Poke2Id;
        public int Poke2Move;
        public int Poke3Id;
        public int Poke3Move;
        public int Poke4Id;
        public int Poke4Move;
        public int Poke5Id;
        public int Poke5Move;
    }

    public interface IGameCreateCommand : ICommand<GameCreateRequest>
    {
    }

    public class GameCreate : IGameCreateCommand
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
                var game = new PogoGamePokes {  
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