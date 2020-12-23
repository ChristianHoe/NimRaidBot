using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBot.DataAccess.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Helper
{
    public class Business
    {
        public static async Task SendGymList(IEnumerable<PogoGyms> gyms, IEnumerable<NotifyLocation> notifications, long chatId, TelegramBotClient bot)
        {
            const string CHECK = "\u2705";

            StringBuilder msg = new StringBuilder("Bitte wähle ein Gym" + Environment.NewLine);

            int i = 0;
            foreach (var gym in gyms)
            {
                var mark = notifications.Any(x => x.LocationId == gym.Id) ? $"{CHECK} " : string.Empty;
                var line = $"{i} - {mark}{gym.Name}";
                if (line.Length + msg.Length > 4096)
                {
                    await bot.SendTextMessageAsync(chatId, msg.ToString());
                    msg.Clear();
                }

                msg.AppendLine(line);
                i++;
            }

            await bot.SendTextMessageAsync(chatId, msg.ToString());
        }

 
        public static async Task<int?> GetGymId(string text, IEnumerable<PogoGyms> gyms, ChatId chatId, TelegramBotClient bot)
        {
            if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int gymIndex))
            {
                var names = gyms.Where(x => x.Name?.Contains(text, StringComparison.InvariantCultureIgnoreCase) ?? false);
                if (names.Count() == 0)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return  null;
                }

                if (names.Count() > 1)
                {
                    var msg = "Gym nicht eindeutig!" + Environment.NewLine + string.Join(Environment.NewLine, names.Select(x => $"Name: {x.Name}"));
                    await bot.SendTextMessageAsync(chatId, msg).ConfigureAwait(false);
                    return null;
                }

                return names.First().Id;
            }
            else
            {
                if (gymIndex < 0 || gyms.Count() < gymIndex)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return null;
                }
                
                return gyms.ElementAt(gymIndex).Id;
            }
        }
    }
}