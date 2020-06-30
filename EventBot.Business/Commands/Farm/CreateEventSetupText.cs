using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Farm
{
    public class CreateEventSetupTextRequest
    {
        //public TmpEvents EventSetup;
        public Locations Locations;
        public DateTime? Start;
        public Eventtypes Eventtypes;
    }

    public class CreateEventSetupTextResponse
    {
        public string Text;
        public InlineKeyboardMarkup InlineKeyboardMarkup;
        public ParseMode ParseMode;
    }

    public interface ICreateEventSetupText
    {
        CreateEventSetupTextResponse Execute(CreateEventSetupTextRequest request);
    }

    public class CreateEventSetupText : ICreateEventSetupText
    {
        const string TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";

        public CreateEventSetupTextResponse Execute(CreateEventSetupTextRequest request)
        {
            var text = new StringBuilder("Event-Setup\r\n");
            text.AppendLine($"Typ: {request.Eventtypes.Name}");
            text.AppendLine($"Start: {request.Start?.ToString(TIME_FORMAT)}");
            text.AppendLine($"Ort: {request.Locations?.Name}");

            //var chat = new[] { new InlineKeyboardCallbackButton("<< Chat", "c|-1"), new InlineKeyboardCallbackButton("Chat >>", "c|1") };
            //var keyBoardEvent = new[] { new InlineKeyboardCallbackButton("<< Event", "e|-1"), new InlineKeyboardCallbackButton("Event >>", "e|1") };
            var keyBoardLocation = new[] { new InlineKeyboardButton { Text = "<< Loc", CallbackData = "l|-1" }, new InlineKeyboardButton { Text = "Loc >>", CallbackData = "l|1" } };
            var keyBoardDay = new[] { new InlineKeyboardButton { Text = "-1d", CallbackData = "d|-1" }, new InlineKeyboardButton { Text = "+1d", CallbackData = "d|1"} };
            var keyBoardTime = new[] { new InlineKeyboardButton { Text = "-30m", CallbackData = "m|-30" }, new InlineKeyboardButton { Text = "+30m", CallbackData = "m|+30"} };
            var generate = new[] { new InlineKeyboardButton{ Text = "Erzeugen", CallbackData =  "c|1"} };

            var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[4][] { /*chat, keyBoardEvent,*/ keyBoardLocation, keyBoardDay, keyBoardTime, generate });

            return new CreateEventSetupTextResponse { Text = text.ToString(), InlineKeyboardMarkup = inlineKeyboard, ParseMode = ParseMode.Markdown };
        }
    }
}
