using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.Business.Intrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Tasks
{
    public abstract class BaseMessageProcessor : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly BaseDispatcher dispatcher;

        readonly private Queries.StatePeakQuery lastStateQuery;
        readonly private DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand;

        public BaseMessageProcessor(
            TelegramBotClient proxy,
            BaseDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            )
        {
            this.proxy = proxy;
            this.dispatcher = dispatcher;
            this.lastStateQuery = lastStateQuery;
            this.modifyChatTitleCommand = modifyChatTitleCommand;
        }

        public abstract string Name { get; }

        public bool RunExclusive => true;
        public abstract int IntervallInMilliseconds { get; }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.ProcessMessages(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, this.Name + " - ", ex).ConfigureAwait(false);
            }
        }

        private async Task ProcessMessages(CancellationToken cancellationToken)
        {
            // Used for getting only the unconfirmed updates.
            // It is recommended to stored this value between sessions. 
            // More information at https://core.telegram.org/bots/api#getting-updates
            var offset = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    Update[] updates = await proxy.GetUpdatesAsync(offset).ConfigureAwait(false);

                    if (updates.Any())
                    {
                        offset = updates.Max(u => u.Id) + 1;

                        foreach (Update update in updates)
                        {
                            try
                            {
                                switch (update.Type)
                                {
                                    case UpdateType.Message:
                                        await this.CheckMessagesAsync(update.Message).ConfigureAwait(false);
                                        break;
                                    case UpdateType.CallbackQuery:
                                        var result = await this.CheckCallbacksAsync(update.CallbackQuery).ConfigureAwait(false);
                                        try
                                        {
                                            await proxy.AnswerCallbackQueryAsync(update.CallbackQuery.Id, result.NotificationMessage, result.NotifiyUser).ConfigureAwait(false);
                                        }
                                        catch
                                        {
                                            // to slow
                                        }
                                        break;
                                    case UpdateType.InlineQuery:
                                        await this.AnswerInlineQueryAsync(update.InlineQuery).ConfigureAwait(false);
                                        break;
                                    case UpdateType.ChosenInlineResult:
                                        await this.ChosenInlineResultAsync(update.ChosenInlineResult).ConfigureAwait(false);
                                        break;
                                    case UpdateType.ChannelPost:
                                        // do not know what to do about
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            catch (Exception ex)
                            {
                                await Operator.SendMessage(this.proxy, this.Name + ":  " + update.Message.Text + " - ", ex).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
                }
                //else
                //{
                //    // polling is done extern via Cache-Invalidation
                //    break;
                //}

                await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
            }

            return;
        }

        private async Task<AnswerResult> CheckCallbacksAsync(CallbackQuery callbackQuery)
        {
            var answer = this.dispatcher.GetAnswer(callbackQuery);
            if (answer != null)
            {
                return await answer.ExecuteAsync(callbackQuery, callbackQuery.Data, proxy).ConfigureAwait(false);
            }

            return new AnswerResult();
        }

        private async Task AnswerInlineQueryAsync(InlineQuery inlineQuery)
        {
            await proxy.AnswerInlineQueryAsync(inlineQuery.Id, new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultBase[] {
                new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("1", "1111", new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("2", "2222", new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("3", "1111",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("4", "2222",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("5", "1111",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("6", "2222",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("7", "1111",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("8", "2222",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("9", "1111",  new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("10", "2222", new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("11", "1111", new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber1"))
                ,new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle("12", "2222", new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("laber2"))
            }, isPersonal: true).ConfigureAwait(false);
            return;
        }

        private Task ChosenInlineResultAsync(ChosenInlineResult chosenInlineResult)
        {
            return Task.CompletedTask;
        }

        private async Task CheckMessagesAsync(Message message)
        {
            if (message == null)
                return;

            var commandTexts = message.Entities == null ? Enumerable.Empty<MessageEntity>() : message.Entities.Where(x => x.Type == MessageEntityType.BotCommand);
            if (commandTexts.Count() > 1)
            {
                await proxy.SendTextMessageAsync(message.Chat.Id, "Zuviele Befehle auf einmal").ConfigureAwait(false);
                return;
            }

            // if new command => run command
            if (commandTexts.Count() == 1)
            {
                var msg = message.Text;
                var commandText = string.Empty;
                for (int i = message.Entities.Count()-1; i >= 0; i--)
                {
                    if (message.Entities[i].Type == MessageEntityType.BotCommand)
                    {
                        var cmd = msg.Substring(message.Entities[i].Offset, message.Entities[i].Length).ToLower();
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            var cmds = cmd.Split("_");

                            commandText = cmds[0];

                            msg = msg.Replace(commandText + (cmds.Count() == 1 ? string.Empty : "_"), string.Empty, StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                    else
                    {
                        msg = msg.Remove(message.Entities[i].Offset, message.Entities[i].Length);
                    }
                }

                if (!this.dispatcher.CommandExists(commandText))
                {
                    return;
                    //x = "/help";
                }

                var command = this.dispatcher.GetCommand(commandText);

                await command.ExecuteAsync(message, msg, proxy);

                return;
            }

            if (message.NewChatMembers != null)
            {
                var memberAdded = this.dispatcher.MemberAdded();
                if (memberAdded != null)
                {
                    await memberAdded.Execute(message, this.proxy, this.proxy.BotId).ConfigureAwait(false);
                    return;
                }

                return;
            }

            if (message.LeftChatMember != null)
            {
                var memberRemoved = this.dispatcher.MemberRemoved();
                if (memberRemoved != null)
                {
                    await memberRemoved.Execute(message, this.proxy, this.proxy.BotId).ConfigureAwait(false);
                    return;
                }

                return;
            }

            if (message.NewChatPhoto != null)
            {
                return;
            }

            if (message.NewChatTitle != null)
            {
                this.modifyChatTitleCommand.Execute(new DataAccess.Commands.Raid.ModifyChatTitleRequest { ChatId = message.Chat.Id, Name = message.NewChatTitle });
                return;
            }

            // if just text => check for registered state/command => run command
            var state = this.lastStateQuery.Execute(message);
            if (state != null)
            {
                var command = this.dispatcher.GetStatefulCommand(state.Command);
                if (command != null)
                    await command.ExecuteStepAsync(message, message.Text, proxy, state.Step);

                return;
            }

            // check for state-less reply
            if (message.ReplyToMessage != null)
            {
                var command = this.dispatcher.GetCommand("[RaidBossName]");

                await command.ExecuteAsync(message, message.Text, proxy);
            }

            //if (message.Entities == null || !message.Entities.Any(x => x.Type == MessageEntityType.BotCommand))
            //{
            //    await proxy.SendTextMessageAsync(message.Chat.Id, "Bitte benutzen Sie einen der Befehle, die Sie unter /help finden.").ConfigureAwait(false);
            //    return;
            //}
        }
    }
}
