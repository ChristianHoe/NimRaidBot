using EventBot.Business.Helper;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.Base;
using EventBot.DataAccess.Queries.PoGo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.Raid
{
    public class SendRaidPollRequest
    {
        public IEnumerable<PollVoteResponse> Votes;
        public DataAccess.Queries.Raid.Raid Raid;
        public Dictionary<int, string> PokeNames;
        public IEnumerable<PogoSpecialGyms> SpecialGymSettings;
        public IEnumerable<PogoRaidPreference> RaidPreferences;
        public IEnumerable<int> TimeOffsets;
    }

    public class RaidPollResponse
    {
        public string Text;
        public InlineKeyboardMarkup InlineKeyboardMarkup;
        public ParseMode ParseMode;
    }

    public interface ICreatePollText
    {
        RaidPollResponse Execute(SendRaidPollRequest request);
    }

    public class CreatePollText : ICreatePollText
    {
        const string PIN = "\U0001F4CD";
        const string PIN2 = "\U0001F4CC";
        const string GEM = "\U0001F48E";
        const string CLOCK = "\U000023F1";
        const string CLOCK2 = "\U0001F4C6";

        const string WARNING_SIGN = "\U000026A0";

        const string ONE = "\u0031\u20E3";
        const string TWO = "\u0032\u20E3";
        const string THREE = "\u0033\u20E3";
        const string FOUR = "\u0034\u20E3";
        const string FIVE = "\u0035\u20E3";
        const string M = "\U0001F1F2";
        const string DISALLOWED = "\u1F6AB";

        const string THUMBS_UP = "\U0001F44D";
        const string CROSS_MARK = "\u274C";
        const string SATELLITE = "\U0001F6F0";
        const string ADMISSION_TICKET = "\U0001F39F";

        const string NON_BREAK_SPACE = "\u00A0";

        const string BLUE_HEART = "\U0001F499";
        const string RED_HEART = "\u2764";
        const string YELLOW_HEART = "\U0001F49B";

        const string FIRE = "\U0001F525";
        const string WATER = "\U0001F4A7";
        const string ENERGY = "\u26A1";

        const string CIRCLE = "\u26AA";

        const string HAMSTER = "\U0001F439";
        const string WHALE = "\U0001F433";
        const string SNAKE = "\U0001F40D";

        const string LION_FACE = "\U0001F981";
        const string TIGER_FACE = "\U0001F42F";
        const string WOLF_FACE = "\U0001F43A";

        const string MASKS = "\U0001F3AD";
        const string RAINBOW = "\U0001F308";
        const string VULCAN = "\U0001F30B";
        const string WHALE2 = "\U0001F40B";
        const string DRAGON = "\U0001F409";
        const string BUTTERFLY = "\U0001F98B";
        const string BAT = "\U0001F987";
        const string THUNDER = "\U0001F329";
        const string SPIDER = "\U0001F577";

        const string GEAR_WHEEL = "\u2699";


        const string FISTED_HAND = "\U0001F44A";
        const string BUG = "\U0001F41B";
        const string HERB = "\U0001F33F";
        const string WHITE = "\u26AA";
        const string BLACK = "\u26AB";
        const string DRAGON_FACE = "\U0001F432";
        const string PLANE = "\u2708";
        const string ROCKET = "\U0001F680";
        const string SPARKLES = "\u2728";
        const string GHOST = "\U0001F47B";
        const string SNOW = "\u2744";
        const string POISON = "\u2620";
        const string CYCLON = "\U0001F300";
        const string SNAIL = "\U0001F40C";
        const string MOYAI = "\U0001F5FF";
        const string CHAINS = "\u26D3";
        const string PARROT = "\U0001F99C";

        const string CHRISTMAS_TREE = "\U0001F384";
        const string CHRISTMAS_GIFT = "\U0001F381";
        const string BIRTHDAY_CAKE = "\U0001F382";
        const string BIRTHDAY_CAKE_PIECE = "\U0001F370";

        private readonly IGetRocketMapMovesQuery getRocketMapMovesQuery;

        public CreatePollText(
            IGetRocketMapMovesQuery getRocketMapMovesQuery
            )
        {
            this.getRocketMapMovesQuery = getRocketMapMovesQuery;
        }


        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIds.Local);

        public RaidPollResponse Execute(SendRaidPollRequest request)
        {
            var raid = request.Raid;
            var votes = request.Votes;
            var pokeNames = request.PokeNames;

            var text = new StringBuilder();

            if (string.IsNullOrWhiteSpace(raid.Title))
            {
                text.Append(this.GetRaidLevelSymbol(raid.Level));
                if (raid.PokeId == 0)
                    text.Append(" ?");
                else
                    text.Append($" {pokeNames[raid.PokeId].Trim()}{this.GetForm(raid.PokeForm)}{this.GetMoveTypeSymbols(raid.MoveId)}");
            }
            else
            {
                text.Append(raid.Title);
            }

            text.Append(" " + CLOCK + " ");

            var start = TimeZoneInfo.ConvertTimeFromUtc(raid.Start, timezone);
            text.AppendLine($"{start.ToString("HH:mm:ss")}");

            var gymName = raid.GymName; //.Replace(" ", NON_BREAK_SPACE);
            if (gymName.Length > 22)
                gymName = gymName.Substring(0, 19) + "...";

            if (request.SpecialGymSettings.Any(x => x.Type == (int)GymType.ExRaid))
                text.Append(GEM + NON_BREAK_SPACE);
            else
                text.Append(PIN + NON_BREAK_SPACE);


            text.AppendLine($"[{gymName}](https://maps.google.com/?q={raid.Latitude.ToString(CultureInfo.InvariantCulture)},{raid.Longitude.ToString(CultureInfo.InvariantCulture)})");

            var invitesNeeded = request.Votes.Where(x => LikesInvite(x.Comment)).ToList();
            if (invitesNeeded.Count > 0)
            {
                text.AppendLine();
                text.Append("Einladung gewünscht:");
                foreach(var user in invitesNeeded)
                {
                    text.AppendLine();
                    AppendUser(text, user, request.RaidPreferences);
                }

                text.AppendLine();
            }
            

            if (request.Votes.Where(x => x.Attendee != 0 && !LikesInvite(x.Comment)).Count() == 0)
            {
                //text.AppendLine();
                text.Append("Bisher keine Zusagen");
            }
            else
            {
                var z = votes.Where(x => x.Attendee != 0 && !LikesInvite(x.Comment)).GroupBy(x => x.Time, (key, g) => new { Time = key, Users = g.ToList() }).OrderBy(x => x.Time);
                foreach (var y in z)
                {
                    text.AppendLine();
                    var numberOfAttendee = y.Users.Sum(x => x.Attendee);
                    var numberOfRemotes = y.Users.Where(x => x.Comment == PogoUserVoteComments.Remote).Sum(x => x.Attendee);

                    var numberOfAttendeesFormatted = $"{SATELLITE}{numberOfRemotes}{(numberOfRemotes > 8 ? WARNING_SIGN : string.Empty)} / {numberOfAttendee}{(numberOfAttendee > 18 ? WARNING_SIGN : string.Empty)}";
                    
                    if (string.IsNullOrEmpty(y.Time))
                        text.Append($"Interessenten : {numberOfAttendeesFormatted} ");
                    else
                        text.Append($"{y.Time} - Zusagen : {numberOfAttendeesFormatted} ");

                    foreach (var a in y.Users.OrderBy(b => b.IngameName))
                    {
                        if (a.Attendee != 0)
                        {
                            text.AppendLine();

                            AppendUser(text, a, request.RaidPreferences);
                        }
                    }
                }
            }

            var startFrames = CreateStartTimes(start, request.TimeOffsets, 5).Select(x => x.ToString("HH:mm")).ToArray();
            var keyBoardStartFrames = new InlineKeyboardButton[] {
                 new InlineKeyboardButton { Text = THUMBS_UP, CallbackData = "t|"}
                , new InlineKeyboardButton { Text = startFrames[0], CallbackData = $"t|{startFrames[0]}" }
                , new InlineKeyboardButton { Text = startFrames[1], CallbackData = $"t|{startFrames[1]}" }
                , new InlineKeyboardButton { Text = startFrames[2], CallbackData = $"t|{startFrames[2]}" }
                , new InlineKeyboardButton { Text = startFrames[3], CallbackData = $"t|{startFrames[3]}" }
            };

            // var extendedStartFrames = CreateStartTimes(start.AddMinutes(45), request.TimeOffsets, 5).Select(x => x.ToString("HH:mm")).ToArray();
            // var extendedKeyBoardStartFrames = new InlineKeyboardButton[] {
            //       new InlineKeyboardButton { Text = extendedStartFrames[0], CallbackData = $"t|{extendedStartFrames[0]}" }
            //     , new InlineKeyboardButton { Text = extendedStartFrames[1], CallbackData = $"t|{extendedStartFrames[1]}" }
            //     , new InlineKeyboardButton { Text = extendedStartFrames[2], CallbackData = $"t|{extendedStartFrames[2]}" }
            //     , new InlineKeyboardButton { Text = extendedStartFrames[3], CallbackData = $"t|{extendedStartFrames[3]}" }
            // };

            var keyBoardNumberOfPersons = new[] { 
                  new InlineKeyboardButton { Text = SATELLITE, CallbackData = "c|r" }
                //, new InlineKeyboardButton { Text = THUMBS_UP, CallbackData = "t|"}
                , new InlineKeyboardButton { Text = "-1", CallbackData = "a|-1" }
                , new InlineKeyboardButton { Text = "0", CallbackData = "a|0" }
                , new InlineKeyboardButton { Text = "+1", CallbackData = "a|1" }
                , new InlineKeyboardButton { Text = ADMISSION_TICKET, CallbackData = "c|i"}
                };

            //var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[3][] { keyBoardStartFrames, extendedKeyBoardStartFrames, keyBoardNumberOfPersons });
            var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[2][] { keyBoardStartFrames, keyBoardNumberOfPersons });

            return new RaidPollResponse { Text = text.ToString(), InlineKeyboardMarkup = inlineKeyboard, ParseMode = ParseMode.Markdown };
        }

        private bool LikesInvite(PogoUserVoteComments? c)
        {
            return c.HasValue && (c.Value & PogoUserVoteComments.LikeInvite) != 0;
        }

        private void AppendUser(StringBuilder text, PollVoteResponse a, IEnumerable<PogoRaidPreference> raidPreference)
        {
            if (a.Comment.HasValue)
            {
                if (a.Comment == PogoUserVoteComments.Remote)
                    text.Append($"{SATELLITE} ");
            }
            
            text.Append(" " + GetRaidBossSymbolsForUser(a.UserId, raidPreference));
            text.Append($" [{a.IngameName ?? a.FirstName}](tg://user?id={a.UserId})");
            if (a.Level.HasValue)
                text.Append($" ({a.Level.Value})");
            if (a.Team.HasValue)
                text.Append($" {this.GetTeamColor(a.Team)}");

            if (a.Attendee != 1)
                text.Append($" +{a.Attendee-1}");
        }

        private DateTime[] CreateStartTimes(DateTime date, IEnumerable<int> offsets, int roundToMinute)
        {
            if (roundToMinute > 2)
            {
                date = date.AddMinutes(2);
            }

            var x = date.Minute;
            var y = x / roundToMinute;
            var z = y * roundToMinute;

            var firstDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, z, 0);
            var result = new List<DateTime>();
            foreach (var offset in offsets)
            {
                result.Add(firstDate.AddMinutes(offset));
            }
            return result.ToArray();
        }

        private string GetTeamColor(DataAccess.Commands.Raid.TeamType? teamType)
        {
            if (!teamType.HasValue)
                return string.Empty;

            switch (teamType.Value)
            {
                case DataAccess.Commands.Raid.TeamType.Blau:
                    return WATER; // BLUE_HEART;
                case DataAccess.Commands.Raid.TeamType.Gelb:
                    return ENERGY; // YELLOW_HEART;
                case DataAccess.Commands.Raid.TeamType.Rot:
                    return FIRE; // RED_HEART;
                default:
                    return string.Empty;
            }
        }

        private string GetRaidLevelSymbol(int raidLevel)
        {
            switch (raidLevel)
            {
                case 1:
                    return ONE;
                case 2:
                    return TWO;
                case 3:
                    return THREE;
                case 4:
                    return FOUR;
                case 5:
                    return FIVE;
                case 6:
                    return M;
                default:
                    return DISALLOWED;
            }
        }

        private string GetRaidBossSymbolsForUser(long userId, IEnumerable<PogoRaidPreference> preferences)
        {
            if (preferences == null)
                return string.Empty;

            return string.Join(string.Empty, preferences.Where(x => x.ChatId == userId).OrderBy(x => x.PokeId).Select(x => GetRaidBossSymbols(x.PokeId)));
        }

        private string GetRaidBossSymbols(int pokeId)
        {
            switch(pokeId)
            {
                case 144: // Arktos
                    return BLUE_HEART;
                case 145: // Zapdos
                    return YELLOW_HEART;
                case 146: // Lavados
                    return RED_HEART;
                case 249: // LUGIA
                    return MASKS;
                case 250: // HO-OH
                    return RAINBOW;
                case 243: // Raikou
                    return TIGER_FACE;
                case 244: // Entai
                    return LION_FACE;
                case 245: // Suicune
                    return WOLF_FACE;

                case 377: // REGIS ROCK
                    return MOYAI;
                case 378: // REGIS ICE
                    return SNOW;
                case 379: // REGIS STEEL
                    return GEAR_WHEEL;
                case 380: // Latias
                    return ROCKET;
                case 381: // Latios
                    return PLANE;
                case 382: // Kyogre
                    return WHALE2;
                case 383: // GROUDON
                    return VULCAN;
                case 384: // Rayquasa
                    return DRAGON;
                case 484: //PALKIA
                    return PARROT;
                case 487: // Giratina
                    return BAT;
                case 488: // Cresselia
                    return BUTTERFLY;
                case 491: //Darkrai
                    return SPIDER;
                case 642: // Thunderus
                    return THUNDER;

                default:
                    return string.Empty;
            }
        }

        private string GetForm(char? form)
        {
            if (form == null)
                return string.Empty;

            return " " + form;
        }

        private string GetMoveTypeSymbols(int? moveId)
        {
            if (moveId == null)
                return string.Empty;

            if (this.getRocketMapMovesQuery.Execute(null).TryGetValue(moveId.Value, out Models.RocketMap.Move move))
            {
                switch(move.type)
                {
                    case Models.RocketMap.ElementType.Bug:
                        return BUG;
                    case Models.RocketMap.ElementType.Dark:
                        return BLACK;
                    case Models.RocketMap.ElementType.Dragon:
                        return DRAGON_FACE;
                    case Models.RocketMap.ElementType.Electric:
                        return ENERGY;
                    case Models.RocketMap.ElementType.Fairy:
                        return SPARKLES;
                    case Models.RocketMap.ElementType.Fighting:
                        return FISTED_HAND;
                    case Models.RocketMap.ElementType.Fire:
                        return FIRE;
                    case Models.RocketMap.ElementType.Flying:
                        return PLANE;
                    case Models.RocketMap.ElementType.Ghost:
                        return GHOST;
                    case Models.RocketMap.ElementType.Grass:
                        return HERB;
                    case Models.RocketMap.ElementType.Ground:
                        return SNAIL;
                    case Models.RocketMap.ElementType.Ice:
                        return SNOW;
                    case Models.RocketMap.ElementType.Normal:
                        return WHITE;
                    case Models.RocketMap.ElementType.Poison:
                        return POISON;
                    case Models.RocketMap.ElementType.Psychic:
                        return CYCLON;
                    case Models.RocketMap.ElementType.Rock:
                        return MOYAI;
                    case Models.RocketMap.ElementType.Steel:
                        return CHAINS;
                    case Models.RocketMap.ElementType.Water:
                        return WATER;

                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
