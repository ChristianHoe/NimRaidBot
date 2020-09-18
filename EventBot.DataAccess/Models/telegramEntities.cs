using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EventBot.DataAccess.Models
{
    public partial class telegramEntities : DbContext
    {
        public telegramEntities()
        {
        }

        public telegramEntities(DbContextOptions<telegramEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivePolls> ActivePolls { get; set; }
        public virtual DbSet<ActivePollsMeta> ActivePollsMeta { get; set; }
        public virtual DbSet<BotFarm> BotFarm { get; set; }
        public virtual DbSet<EventSetups> EventSetups { get; set; }
        public virtual DbSet<IngrEvents> IngrEvents { get; set; }
        public virtual DbSet<IngrEventsMeta> IngrEventsMeta { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Memberships> Memberships { get; set; }
        public virtual DbSet<PogoChatPoke> PogoChatPoke { get; set; }
        public virtual DbSet<PogoConfigurations> PogoConfigurations { get; set; }
        public virtual DbSet<PogoGamePokes> PogoGamePokes { get; set; }
        public virtual DbSet<PogoGamePokesAnswers> PogoGamePokesAnswers { get; set; }
        public virtual DbSet<PogoGamePokesMeta> PogoGamePokesMeta { get; set; }
        public virtual DbSet<PogoGyms> PogoGyms { get; set; }
        public virtual DbSet<PogoPokes> PogoPokes { get; set; }
        public virtual DbSet<PogoPokesMeta> PogoPokesMeta { get; set; }
        public virtual DbSet<PogoQuests> PogoQuests { get; set; }
        public virtual DbSet<PogoQuestsHistory> PogoQuestsHistory { get; set; }
        public virtual DbSet<PogoQuestsMeta> PogoQuestsMeta { get; set; }
        public virtual DbSet<PogoRaidPreference> PogoRaidPreference { get; set; }
        public virtual DbSet<PogoRaidTimeOffsets> PogoRaidTimeOffsets { get; set; }
        public virtual DbSet<PogoRaidUsers> PogoRaidUsers { get; set; }
        public virtual DbSet<PogoRaids> PogoRaids { get; set; }
        public virtual DbSet<PogoRaidsMeta> PogoRaidsMeta { get; set; }
        public virtual DbSet<PogoRelPokesChats> PogoRelPokesChats { get; set; }
        public virtual DbSet<PogoRelScanChat> PogoRelScanChat { get; set; }
        public virtual DbSet<PogoScanArea> PogoScanArea { get; set; }
        public virtual DbSet<PogoSpecialGyms> PogoSpecialGyms { get; set; }
        public virtual DbSet<PogoStops> PogoStops { get; set; }
        public virtual DbSet<PogoUser> PogoUser { get; set; }
        public virtual DbSet<PogoUserRaids> PogoUserRaids { get; set; }
        public virtual DbSet<RelChatBot> RelChatBot { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<UserVotes> UserVotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivePolls>(entity =>
            {
                entity.ToTable("ACTIVE_POLLS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Deleted).HasColumnName("DELETED");

                entity.Property(e => e.EventId)
                    .HasColumnName("EVENT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.RaidId)
                    .HasColumnName("RAID_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TimeOffsetId)
                    .HasColumnName("TIME_OFFSET_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ActivePollsMeta>(entity =>
            {
                entity.HasKey(e => e.PollId)
                    .HasName("PRIMARY");

                entity.ToTable("ACTIVE_POLLS_META");

                entity.Property(e => e.PollId)
                    .HasColumnName("POLL_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Farm).HasColumnName("FARM");

                entity.Property(e => e.Poke).HasColumnName("POKE");
            });

            modelBuilder.Entity<BotFarm>(entity =>
            {
                entity.ToTable("BOT_FARM");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BoardId)
                    .HasColumnName("BOARD_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<EventSetups>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY");

                entity.ToTable("EVENT_SETUPS");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.LocationId)
                    .HasColumnName("LOCATION_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Modified).HasColumnName("MODIFIED");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TargetChatId)
                    .HasColumnName("TARGET_CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<IngrEvents>(entity =>
            {
                entity.ToTable("INGR_EVENTS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.LocationId)
                    .HasColumnName("LOCATION_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TypeId)
                    .HasColumnName("TYPE_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<IngrEventsMeta>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PRIMARY");

                entity.ToTable("INGR_EVENTS_META");

                entity.Property(e => e.EventId)
                    .HasColumnName("EVENT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Farm).HasColumnName("FARM");
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.ToTable("LOCATIONS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Order)
                    .HasColumnName("ORDER")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Memberships>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("MEMBERSHIPS");

                entity.Property(e => e.GroupId)
                    .HasColumnName("GROUP_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.LastAccess).HasColumnName("LAST_ACCESS");

                entity.Property(e => e.SecurityLevel)
                    .HasColumnName("SECURITY_LEVEL")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoChatPoke>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.PokeId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_CHAT_POKE");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Gender)
                    .HasColumnName("GENDER")
                    .HasColumnType("char(1)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Iv)
                    .HasColumnName("IV")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoConfigurations>(entity =>
            {
                entity.ToTable("POGO_CONFIGURATIONS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RaidDurationInMin)
                    .HasColumnName("RAID_DURATION_IN_MIN")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoGamePokes>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_GAME_POKES");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice1PokeId)
                    .HasColumnName("CHOICE_1_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice1PokeMoveTyp)
                    .HasColumnName("CHOICE_1_POKE_MOVE_TYP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice2PokeId)
                    .HasColumnName("CHOICE_2_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice2PokeMoveTyp)
                    .HasColumnName("CHOICE_2_POKE_MOVE_TYP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice3PokeId)
                    .HasColumnName("CHOICE_3_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice3PokeMoveTyp)
                    .HasColumnName("CHOICE_3_POKE_MOVE_TYP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice4PokeId)
                    .HasColumnName("CHOICE_4_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Choice4PokeMoveTyp)
                    .HasColumnName("CHOICE_4_POKE_MOVE_TYP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Difficulty)
                    .HasColumnName("DIFFICULTY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Finish).HasColumnName("FINISH");

                entity.Property(e => e.TargetPokeId)
                    .HasColumnName("TARGET_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TargetPokeMoveTyp)
                    .HasColumnName("TARGET_POKE_MOVE_TYP")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoGamePokesAnswers>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_GAME_POKES_ANSWERS");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Choice)
                    .HasColumnName("CHOICE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("USER_NAME")
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoGamePokesMeta>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_GAME_POKES_META");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.State)
                    .HasColumnName("STATE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoGyms>(entity =>
            {
                entity.ToTable("POGO_GYMS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoPokes>(entity =>
            {
                entity.ToTable("POGO_POKES");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Cp)
                    .HasColumnName("CP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.Gender)
                    .HasColumnName("GENDER")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Iv)
                    .HasColumnName("IV")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("decimal(15,13)");

                entity.Property(e => e.Level)
                    .HasColumnName("LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("decimal(15,13)");

                entity.Property(e => e.MapId)
                    .HasColumnName("MAP_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeatherBoosted)
                    .HasColumnName("WEATHER_BOOSTED")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoPokesMeta>(entity =>
            {
                entity.HasKey(e => e.PogoPokeId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_POKES_META");

                entity.Property(e => e.PogoPokeId)
                    .HasColumnName("POGO_POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Poke).HasColumnName("POKE");
            });

            modelBuilder.Entity<PogoQuests>(entity =>
            {
                entity.HasKey(e => e.StopId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_QUESTS");

                entity.Property(e => e.StopId)
                    .HasColumnName("STOP_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Reward)
                    .IsRequired()
                    .HasColumnName("REWARD")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Task)
                    .IsRequired()
                    .HasColumnName("TASK")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoQuestsHistory>(entity =>
            {
                entity.HasKey(e => new { e.StopId, e.Created })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_QUESTS_HISTORY");

                entity.Property(e => e.StopId)
                    .HasColumnName("STOP_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Reward)
                    .IsRequired()
                    .HasColumnName("REWARD")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Task)
                    .IsRequired()
                    .HasColumnName("TASK")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoQuestsMeta>(entity =>
            {
                entity.HasKey(e => e.StopId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_QUESTS_META");

                entity.Property(e => e.StopId)
                    .HasColumnName("STOP_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Processed).HasColumnName("PROCESSED");
            });

            modelBuilder.Entity<PogoRaidPreference>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.PokeId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAID_PREFERENCE");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoRaidTimeOffsets>(entity =>
            {
                entity.HasKey(e => new { e.SettingId, e.Order })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAID_TIME_OFFSETS");

                entity.Property(e => e.SettingId)
                    .HasColumnName("SETTING_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Order)
                    .HasColumnName("ORDER")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OffsetInMinutes)
                    .HasColumnName("OFFSET_IN_MINUTES")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoRaidUsers>(entity =>
            {
                entity.HasKey(e => e.ChatId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAID_USERS");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Active).HasColumnName("ACTIVE");

                entity.Property(e => e.CleanUp)
                    .HasColumnName("CLEAN_UP")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ingress)
                    .IsRequired()
                    .HasColumnName("INGRESS")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.KickInactive).HasColumnName("KICK_INACTIVE");

                entity.Property(e => e.LatMax)
                    .HasColumnName("LAT_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LatMin)
                    .HasColumnName("LAT_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LonMax)
                    .HasColumnName("LON_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LonMin)
                    .HasColumnName("LON_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.MinPokeLevel)
                    .HasColumnName("MIN_POKE_LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RaidLevel)
                    .HasColumnName("RAID_LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RoundToMinute)
                    .HasColumnName("ROUND_TO_MINUTE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TimeOffsetId)
                    .HasColumnName("TIME_OFFSET_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoRaids>(entity =>
            {
                entity.ToTable("POGO_RAIDS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.GymId)
                    .HasColumnName("GYM_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Level)
                    .HasColumnName("LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Move2)
                    .HasColumnName("MOVE2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OwnerId)
                    .HasColumnName("OWNER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PokeForm)
                    .HasColumnName("POKE_FORM")
                    .HasColumnType("varchar(1)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.Title)
                    .HasColumnName("TITLE")
                    .HasColumnType("varchar(40)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoRaidsMeta>(entity =>
            {
                entity.HasKey(e => e.RaidId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAIDS_META");

                entity.Property(e => e.RaidId)
                    .HasColumnName("RAID_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Raid).HasColumnName("RAID");
            });

            modelBuilder.Entity<PogoRelPokesChats>(entity =>
            {
                entity.ToTable("POGO_REL_POKES_CHATS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnName("DELETED")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.MessageId)
                    .HasColumnName("MESSAGE_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<PogoRelScanChat>(entity =>
            {
                entity.HasKey(e => new { e.ScanAreaId, e.ChatId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_REL_SCAN_CHAT");

                entity.Property(e => e.ScanAreaId)
                    .HasColumnName("SCAN_AREA_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<PogoScanArea>(entity =>
            {
                entity.ToTable("POGO_SCAN_AREA");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasColumnName("ACTIVE")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LatMax)
                    .HasColumnName("LAT_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LatMin)
                    .HasColumnName("LAT_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LonMax)
                    .HasColumnName("LON_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LonMin)
                    .HasColumnName("LON_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.MapId)
                    .HasColumnName("MAP_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoSpecialGyms>(entity =>
            {
                entity.HasKey(e => new { e.GymId, e.ChatId })
                    .HasName("PRIMARY");

                entity.ToTable("POGO_SPECIAL_GYMS");

                entity.Property(e => e.GymId)
                    .HasColumnName("GYM_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Data)
                    .HasColumnName("DATA")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoStops>(entity =>
            {
                entity.ToTable("POGO_STOPS");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PogoUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_USER");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Active).HasColumnName("ACTIVE");

                entity.Property(e => e.FirstName)
                    .HasColumnName("FIRST_NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.GroupMembers)
                    .HasColumnName("GROUP_MEMBERS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IngameName)
                    .HasColumnName("INGAME_NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IngressName)
                    .HasColumnName("INGRESS_NAME")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LatMax)
                    .HasColumnName("LAT_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LatMin)
                    .HasColumnName("LAT_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Level)
                    .HasColumnName("LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LonMax)
                    .HasColumnName("LON_MAX")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.LonMin)
                    .HasColumnName("LON_MIN")
                    .HasColumnType("decimal(12,9)");

                entity.Property(e => e.Team)
                    .HasColumnName("TEAM")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<PogoUserRaids>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_USER_RAIDS");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.GymId)
                    .HasColumnName("GYM_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Level)
                    .HasColumnName("LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PokeForm)
                    .HasColumnName("POKE_FORM")
                    .HasColumnType("varchar(1)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PokeId)
                    .HasColumnName("POKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RaidId)
                    .HasColumnName("RAID_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TimeMode)
                    .HasColumnName("TIME_MODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .HasColumnName("TITLE")
                    .HasColumnType("varchar(40)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdRaidId)
                    .HasColumnName("UPD_RAID_ID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<RelChatBot>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.BotId })
                    .HasName("PRIMARY");

                entity.ToTable("REL_CHAT_BOT");

                entity.Property(e => e.ChatId)
                    .HasColumnName("CHAT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.BotId)
                    .HasColumnName("BOT_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AllowNotification).HasColumnName("ALLOW_NOTIFICATION");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Level })
                    .HasName("PRIMARY");

                entity.ToTable("STATES");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Level)
                    .HasColumnName("LEVEL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasColumnName("COMMAND")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Step)
                    .HasColumnName("STEP")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<UserVotes>(entity =>
            {
                entity.HasKey(e => new { e.PollId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("USER_VOTES");

                entity.Property(e => e.PollId)
                    .HasColumnName("POLL_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("USER_ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Attendee)
                    .HasColumnName("ATTENDEE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comment)
                    .HasColumnName("COMMENT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Time)
                    .HasColumnName("TIME")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
