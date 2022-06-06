using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBot.Business.Commands.Minun;
using EventBot.Business.Helper;
using EventBot.DataAccess.Queries.Minun;
using Telegram.Bot;

namespace EventBot.Business.Tasks.MinunBot
{
    public sealed class MinumBotGameAnnouncer : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly IGetCurrentGamesQuery getCurrentGamesQuery;
        private readonly IGetGameAnswersQuery getGameAnswersQuery;
        private readonly IGamePokeCreateText gamePokeCreateText;

        public MinumBotGameAnnouncer(
            TelegramProxies.MinunBot proxy,
            IGetCurrentGamesQuery getCurrentGamesQuery,
            IGetGameAnswersQuery getGameAnswersQuery,
            IGamePokeCreateText gamePokeCreateText

            )
        {
            this.proxy = proxy;
            this.getGameAnswersQuery = getGameAnswersQuery;
            this.gamePokeCreateText = gamePokeCreateText;
            this.getCurrentGamesQuery = getCurrentGamesQuery;
        }

        public string Name { get { return "MinumBot.GameAnnouncer"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                var games = this.getCurrentGamesQuery.Execute(new GetCurrentGamesRequest(Until: now.AddSeconds(-20)));


                foreach(var game in games)
                {
                    try
                    {
                        if (game.Finish.AddSeconds(-5) < now || game.Finish.AddSeconds(-25) > now)
                        {
                        var answers = this.getGameAnswersQuery.Execute(new GetGameAnswersRequest(ChatId: game.ChatId, MessageId: game.MessageId)).ToArray();
                        var response = this.gamePokeCreateText.Execute(new GamePokeCreateTextRequest(Now: now, Game: game, Votes: answers));
                                
                        await proxy.EditMessageTextAsync(game.ChatId, game.MessageId, response.Text, parseMode: response.ParseMode, replyMarkup: response.InlineKeyboardMarkup, disableWebPagePreview: true).ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        //ignore 
                    }
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, this.Name + " - ", ex).ConfigureAwait(false);
            }
        }
    }
}