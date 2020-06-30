﻿// using EventBot.Business.Interfaces;
// using EventBot.DataAccess.Commands.Raid;
// using EventBot.DataAccess.Queries.Raid;
// using System.Globalization;
// using System.Threading.Tasks;
// using Telegram.Bot;
// using Telegram.Bot.Types;

// namespace EventBot.Business.Commands.Raid
// {
//     public interface IConfigureUserCommand : ICommand
//     { }

//     public class ConfigureUserCommand : StatefulCommand, IConfigureUserCommand
//     {
//         private readonly IGetCurrentUserSettingsQuery getCurrentUserSettingsQuery;
//         private readonly IAddUserCommand addUserCommand;
//         private readonly ISetUserLevelCommand setUserLevelCommand;
//         private readonly ISetUserNameCommand setUserNameCommand;
//         private readonly ISetUserTeamCommand setUserTeamCommand;

//         public ConfigureUserCommand(
//             DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
//             DataAccess.Commands.IStatePopCommand statePopCommand,
//             DataAccess.Commands.IStatePushCommand statePushCommand,
//             Queries.StatePeakQuery statePeakQuery,

//             IAddUserCommand addUserCommand,
//             IGetCurrentUserSettingsQuery getCurrentUserSettingsQuery,
//             ISetUserLevelCommand setUserLevelCommand,
//             ISetUserNameCommand setUserNameCommand,
//             ISetUserTeamCommand setUserTeamCommand

//             )
//             : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
//         {
//             this.addUserCommand = addUserCommand;
//             this.getCurrentUserSettingsQuery = getCurrentUserSettingsQuery;
//             this.setUserLevelCommand = setUserLevelCommand;
//             this.setUserNameCommand = setUserNameCommand;
//             this.setUserTeamCommand = setUserTeamCommand;


//             base.Steps.Add(0, this.Step0);

//             base.Steps.Add(1, this.Step1);
//             base.Steps.Add(2, this.Step2);


//             base.Steps.Add(3, this.Step3);
//             base.Steps.Add(4, this.Step4);


//             base.Steps.Add(5, this.Step5);
//             base.Steps.Add(6, this.Step6);

//         }

//         public override string HelpText => "Startet Nutzereinrichtung";
//         public override string Key => "/user_settings";
//         public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

//         protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var chatId = base.GetChatId(message);

//             await bot.SendTextMessageAsync(chatId, "Willkommen bei der Einrichtung des Bots").ConfigureAwait(false);
//             await bot.SendTextMessageAsync(chatId, "Du kannst 'x' antworten, dann wird der ursprüngliche Wert beibehalten.").ConfigureAwait(false);

//             return await this.Step1(message, text, bot, step);
//         }

//         protected async Task<bool> Step1(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var userId = base.GetUserId(message);
//             var chatId = base.GetChatId(message);

//             var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest { UserId = userId });

//             var name = currentSettings.IngameName ?? "-";

//             await bot.SendTextMessageAsync(chatId, $"Aktueller Wert {name} \n\rNenne deinen PoGo-Namen");
//             base.NextState(message, 2);
//             return false;
//         }

//         protected async Task<bool> Step2(Message message, string text, TelegramBotClient bot, int step)
//         {
//             if (!SkipCurrentStep(text))
//             {
//                 var userId = base.GetUserId(message);

//                 this.setUserNameCommand.Execute(new DataAccess.Commands.Raid.SetUserNameRequest { UserId = userId, Name = text });
//             }

//             return await this.Step3(message, text, bot, step);
//         }

//         protected async Task<bool> Step3(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var userId = base.GetUserId(message);
//             var chatId = base.GetChatId(message);

//             var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest { UserId = userId });
//             var team = currentSettings.Team?.ToString() ?? "-";

//             await bot.SendTextMessageAsync(chatId, $"Aktueller Wert {team} \n\rWelches Team bist du 1-Blau, 2-Rot, 3-Gelb?");

//             base.NextState(message, 4);
//             return false;
//         }

//         protected async Task<bool> Step4(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var userId = base.GetUserId(message);
//             var chatId = base.GetChatId(message);

//             if (!SkipCurrentStep(text))
//             {
//                 if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int team))
//                 {
//                     await bot.SendTextMessageAsync(chatId, "Das Team konnte nicht erkannt werden, bitte probiere es noch einmal.");
//                     return false;
//                 }

//                 if ( team < 1 || 3 < team)
//                 {
//                     await bot.SendTextMessageAsync(chatId, "Das Team konnte nicht erkannt werden, bitte probiere es noch einmal.");
//                     return false;
//                 }

//                 this.setUserTeamCommand.Execute(new DataAccess.Commands.Raid.SetUserTeamRequest { UserId = userId, Team = (TeamType)team });
//             }

//             return await this.Step5(message, text, bot, step);
//         }

//         protected async Task<bool> Step5(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var userId = base.GetUserId(message);
//             var chatId = base.GetChatId(message);

//             var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest { UserId = userId });

//             var level = currentSettings.Level?.ToString() ?? "-";

//             await bot.SendTextMessageAsync(chatId, $"Aktueller Wert {level} \n\rWelches Level bist du?");
//             base.NextState(message, 6);
//             return false;
//         }

//         protected async Task<bool> Step6(Message message, string text, TelegramBotClient bot, int step)
//         {
//             var userId = base.GetUserId(message);
//             var chatId = base.GetChatId(message);

//             if (!SkipCurrentStep(text))
//             {
//                 if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int level))
//                 {
//                     await bot.SendTextMessageAsync(chatId, "Das Level konnte nicht erkannt werden, bitte probiere es noch einmal.");
//                     return false;
//                 }

//                 if (level < 1 || 40 < level)
//                 {
//                     await bot.SendTextMessageAsync(chatId, "Das Level konnte nicht erkannt werden, bitte probiere es noch einmal.");
//                     return false;
//                 }

//                 this.setUserLevelCommand.Execute(new DataAccess.Commands.Raid.SetUserLevelRequest { UserId = userId, Level = level });
//             }

//             await bot.SendTextMessageAsync(chatId, "Konfiguration abgeschlossen");

//             return true;
//         }

//         private bool SkipCurrentStep(string text)
//         {
//             // geht eigentlich nicht
//             if (string.IsNullOrWhiteSpace(text))
//                 return true;

//             return text.Trim().ToLowerInvariant().Equals("x");
//         }
//     }
// }
