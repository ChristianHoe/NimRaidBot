using System;
using System.Collections.Generic;
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

        public virtual DbSet<ActivePoll> ActivePolls { get; set; } = null!;
        public virtual DbSet<ActivePollsMetum> ActivePollsMeta { get; set; } = null!;
        public virtual DbSet<BotFarm> BotFarms { get; set; } = null!;
        public virtual DbSet<EventSetup> EventSetups { get; set; } = null!;
        public virtual DbSet<IngrEvent> IngrEvents { get; set; } = null!;
        public virtual DbSet<IngrEventsMetum> IngrEventsMeta { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Membership> Memberships { get; set; } = null!;
        public virtual DbSet<NotifyLocation> NotifyLocations { get; set; } = null!;
        public virtual DbSet<PogoChatPoke> PogoChatPokes { get; set; } = null!;
        public virtual DbSet<PogoConfiguration> PogoConfigurations { get; set; } = null!;
        public virtual DbSet<PogoGamePoke> PogoGamePokes { get; set; } = null!;
        public virtual DbSet<PogoGamePokesAnswer> PogoGamePokesAnswers { get; set; } = null!;
        public virtual DbSet<PogoGamePokesMetum> PogoGamePokesMeta { get; set; } = null!;
        public virtual DbSet<PogoGym> PogoGyms { get; set; } = null!;
        public virtual DbSet<PogoPoke> PogoPokes { get; set; } = null!;
        public virtual DbSet<PogoPokesMetum> PogoPokesMeta { get; set; } = null!;
        public virtual DbSet<PogoQuest> PogoQuests { get; set; } = null!;
        public virtual DbSet<PogoQuestsHistory> PogoQuestsHistories { get; set; } = null!;
        public virtual DbSet<PogoQuestsMetum> PogoQuestsMeta { get; set; } = null!;
        public virtual DbSet<PogoRaid> PogoRaids { get; set; } = null!;
        public virtual DbSet<PogoRaidPreference> PogoRaidPreferences { get; set; } = null!;
        public virtual DbSet<PogoRaidTimeOffset> PogoRaidTimeOffsets { get; set; } = null!;
        public virtual DbSet<PogoRaidUser> PogoRaidUsers { get; set; } = null!;
        public virtual DbSet<PogoRaidsMetum> PogoRaidsMeta { get; set; } = null!;
        public virtual DbSet<PogoRelPokesChat> PogoRelPokesChats { get; set; } = null!;
        public virtual DbSet<PogoRelScanChat> PogoRelScanChats { get; set; } = null!;
        public virtual DbSet<PogoScanArea> PogoScanAreas { get; set; } = null!;
        public virtual DbSet<PogoSpecialGym> PogoSpecialGyms { get; set; } = null!;
        public virtual DbSet<PogoStop> PogoStops { get; set; } = null!;
        public virtual DbSet<PogoUser> PogoUsers { get; set; } = null!;
        public virtual DbSet<PogoUserRaid> PogoUserRaids { get; set; } = null!;
        public virtual DbSet<RelChatBot> RelChatBots { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<UserVote> UserVotes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<ActivePoll>(entity =>
            {
                entity.ToTable("ACTIVE_POLLS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Deleted).HasColumnName("DELETED");

                entity.Property(e => e.EventId)
                    .HasColumnType("int(11)")
                    .HasColumnName("EVENT_ID");

                entity.Property(e => e.MessageId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.RaidId)
                    .HasColumnType("int(11)")
                    .HasColumnName("RAID_ID");

                entity.Property(e => e.TimeOffsetId)
                    .HasColumnType("int(11)")
                    .HasColumnName("TIME_OFFSET_ID");
            });

            modelBuilder.Entity<ActivePollsMetum>(entity =>
            {
                entity.HasKey(e => e.PollId)
                    .HasName("PRIMARY");

                entity.ToTable("ACTIVE_POLLS_META");

                entity.Property(e => e.PollId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("POLL_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Farm).HasColumnName("FARM");

                entity.Property(e => e.Poke).HasColumnName("POKE");
            });

            modelBuilder.Entity<BotFarm>(entity =>
            {
                entity.ToTable("BOT_FARM");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.BoardId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("BOARD_ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");
            });

            modelBuilder.Entity<EventSetup>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("EVENT_SETUPS");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.MessageId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(11)")
                    .HasColumnName("LOCATION_ID");

                entity.Property(e => e.Modified).HasColumnName("MODIFIED");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TargetChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("TARGET_CHAT_ID");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasColumnName("TYPE");
            });

            modelBuilder.Entity<IngrEvent>(entity =>
            {
                entity.ToTable("INGR_EVENTS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(11)")
                    .HasColumnName("LOCATION_ID");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TypeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("TYPE_ID");
            });

            modelBuilder.Entity<IngrEventsMetum>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PRIMARY");

                entity.ToTable("INGR_EVENTS_META");

                entity.Property(e => e.EventId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("EVENT_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Farm).HasColumnName("FARM");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("LOCATIONS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Latitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Longitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("NAME");

                entity.Property(e => e.Order)
                    .HasColumnType("int(11)")
                    .HasColumnName("ORDER");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasColumnName("TYPE");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("MEMBERSHIPS");

                entity.Property(e => e.GroupId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("GROUP_ID");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.LastAccess).HasColumnName("LAST_ACCESS");

                entity.Property(e => e.SecurityLevel)
                    .HasColumnType("int(11)")
                    .HasColumnName("SECURITY_LEVEL");
            });

            modelBuilder.Entity<NotifyLocation>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.LocationId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("NOTIFY_LOCATION");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.LocationId)
                    .HasColumnType("int(11)")
                    .HasColumnName("LOCATION_ID");
            });

            modelBuilder.Entity<PogoChatPoke>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.PokeId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_CHAT_POKE");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POKE_ID");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .HasColumnName("GENDER")
                    .IsFixedLength();

                entity.Property(e => e.Iv)
                    .HasColumnType("int(11)")
                    .HasColumnName("IV");
            });

            modelBuilder.Entity<PogoConfiguration>(entity =>
            {
                entity.ToTable("POGO_CONFIGURATIONS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.RaidDurationInMin)
                    .HasColumnType("int(11)")
                    .HasColumnName("RAID_DURATION_IN_MIN");
            });

            modelBuilder.Entity<PogoGamePoke>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_GAME_POKES");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.MessageId)
                    .HasColumnType("int(11)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.Choice1PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_1_POKE_ID");

                entity.Property(e => e.Choice1PokeMoveTyp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_1_POKE_MOVE_TYP");

                entity.Property(e => e.Choice2PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_2_POKE_ID");

                entity.Property(e => e.Choice2PokeMoveTyp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_2_POKE_MOVE_TYP");

                entity.Property(e => e.Choice3PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_3_POKE_ID");

                entity.Property(e => e.Choice3PokeMoveTyp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_3_POKE_MOVE_TYP");

                entity.Property(e => e.Choice4PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_4_POKE_ID");

                entity.Property(e => e.Choice4PokeMoveTyp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE_4_POKE_MOVE_TYP");

                entity.Property(e => e.Difficulty)
                    .HasColumnType("int(11)")
                    .HasColumnName("DIFFICULTY");

                entity.Property(e => e.Finish).HasColumnName("FINISH");

                entity.Property(e => e.TargetPokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("TARGET_POKE_ID");

                entity.Property(e => e.TargetPokeMoveTyp)
                    .HasColumnType("int(11)")
                    .HasColumnName("TARGET_POKE_MOVE_TYP");
            });

            modelBuilder.Entity<PogoGamePokesAnswer>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId, e.UserId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.ToTable("POGO_GAME_POKES_ANSWERS");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.MessageId)
                    .HasColumnType("int(11)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Choice)
                    .HasColumnType("int(11)")
                    .HasColumnName("CHOICE");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.UserName)
                    .HasMaxLength(25)
                    .HasColumnName("USER_NAME");
            });

            modelBuilder.Entity<PogoGamePokesMetum>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.MessageId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_GAME_POKES_META");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.MessageId)
                    .HasColumnType("int(11)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.State)
                    .HasColumnType("int(11)")
                    .HasColumnName("STATE");
            });

            modelBuilder.Entity<PogoGym>(entity =>
            {
                entity.ToTable("POGO_GYMS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Latitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Longitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<PogoPoke>(entity =>
            {
                entity.ToTable("POGO_POKES");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Cp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CP");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.Gender)
                    .HasColumnType("int(11)")
                    .HasColumnName("GENDER");

                entity.Property(e => e.Iv)
                    .HasColumnType("int(11)")
                    .HasColumnName("IV");

                entity.Property(e => e.Latitude)
                    .HasPrecision(15, 13)
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasColumnName("LEVEL");

                entity.Property(e => e.Longitude)
                    .HasPrecision(15, 13)
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.MapId)
                    .HasColumnType("int(11)")
                    .HasColumnName("MAP_ID");

                entity.Property(e => e.PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POKE_ID");

                entity.Property(e => e.WeatherBoosted)
                    .HasColumnType("int(11)")
                    .HasColumnName("WEATHER_BOOSTED");
            });

            modelBuilder.Entity<PogoPokesMetum>(entity =>
            {
                entity.HasKey(e => e.PogoPokeId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_POKES_META");

                entity.Property(e => e.PogoPokeId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("POGO_POKE_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Poke).HasColumnName("POKE");
            });

            modelBuilder.Entity<PogoQuest>(entity =>
            {
                entity.HasKey(e => e.StopId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_QUESTS");

                entity.Property(e => e.StopId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("STOP_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Reward)
                    .HasMaxLength(200)
                    .HasColumnName("REWARD");

                entity.Property(e => e.Task)
                    .HasMaxLength(200)
                    .HasColumnName("TASK");
            });

            modelBuilder.Entity<PogoQuestsHistory>(entity =>
            {
                entity.HasKey(e => new { e.StopId, e.Created })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_QUESTS_HISTORY");

                entity.Property(e => e.StopId)
                    .HasColumnType("int(11)")
                    .HasColumnName("STOP_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Reward)
                    .HasMaxLength(200)
                    .HasColumnName("REWARD");

                entity.Property(e => e.Task)
                    .HasMaxLength(200)
                    .HasColumnName("TASK");
            });

            modelBuilder.Entity<PogoQuestsMetum>(entity =>
            {
                entity.HasKey(e => e.StopId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_QUESTS_META");

                entity.Property(e => e.StopId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("STOP_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Processed).HasColumnName("PROCESSED");
            });

            modelBuilder.Entity<PogoRaid>(entity =>
            {
                entity.ToTable("POGO_RAIDS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Finished).HasColumnName("FINISHED");

                entity.Property(e => e.GymId)
                    .HasColumnType("int(11)")
                    .HasColumnName("GYM_ID");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasColumnName("LEVEL");

                entity.Property(e => e.Move2)
                    .HasColumnType("int(11)")
                    .HasColumnName("MOVE2");

                entity.Property(e => e.OwnerId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("OWNER_ID");

                entity.Property(e => e.PokeForm)
                    .HasMaxLength(1)
                    .HasColumnName("POKE_FORM");

                entity.Property(e => e.PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POKE_ID");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.Title)
                    .HasMaxLength(40)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<PogoRaidPreference>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.PokeId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_RAID_PREFERENCE");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POKE_ID");
            });

            modelBuilder.Entity<PogoRaidTimeOffset>(entity =>
            {
                entity.HasKey(e => new { e.SettingId, e.Order })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_RAID_TIME_OFFSETS");

                entity.Property(e => e.SettingId)
                    .HasColumnType("int(11)")
                    .HasColumnName("SETTING_ID");

                entity.Property(e => e.Order)
                    .HasColumnType("int(11)")
                    .HasColumnName("ORDER");

                entity.Property(e => e.OffsetInMinutes)
                    .HasColumnType("int(11)")
                    .HasColumnName("OFFSET_IN_MINUTES");
            });

            modelBuilder.Entity<PogoRaidUser>(entity =>
            {
                entity.HasKey(e => e.ChatId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAID_USERS");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Active).HasColumnName("ACTIVE");

                entity.Property(e => e.CleanUp)
                    .HasColumnType("int(11)")
                    .HasColumnName("CLEAN_UP");

                entity.Property(e => e.Ingress)
                    .IsRequired()
                    .HasColumnName("INGRESS")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.KickInactive).HasColumnName("KICK_INACTIVE");

                entity.Property(e => e.LatMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MAX");

                entity.Property(e => e.LatMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MIN");

                entity.Property(e => e.LonMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MAX");

                entity.Property(e => e.LonMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MIN");

                entity.Property(e => e.MinPokeLevel)
                    .HasColumnType("int(11)")
                    .HasColumnName("MIN_POKE_LEVEL");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("NAME");

                entity.Property(e => e.RaidLevel)
                    .HasColumnType("int(11)")
                    .HasColumnName("RAID_LEVEL");

                entity.Property(e => e.RoundToMinute)
                    .HasColumnType("int(11)")
                    .HasColumnName("ROUND_TO_MINUTE");

                entity.Property(e => e.TimeOffsetId)
                    .HasColumnType("int(11)")
                    .HasColumnName("TIME_OFFSET_ID");
            });

            modelBuilder.Entity<PogoRaidsMetum>(entity =>
            {
                entity.HasKey(e => e.RaidId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_RAIDS_META");

                entity.Property(e => e.RaidId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("RAID_ID");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.Raid).HasColumnName("RAID");
            });

            modelBuilder.Entity<PogoRelPokesChat>(entity =>
            {
                entity.ToTable("POGO_REL_POKES_CHATS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnName("DELETED")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.MessageId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MESSAGE_ID");

                entity.Property(e => e.PokeId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("POKE_ID");
            });

            modelBuilder.Entity<PogoRelScanChat>(entity =>
            {
                entity.HasKey(e => new { e.ScanAreaId, e.ChatId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_REL_SCAN_CHAT");

                entity.Property(e => e.ScanAreaId)
                    .HasColumnType("int(11)")
                    .HasColumnName("SCAN_AREA_ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");
            });

            modelBuilder.Entity<PogoScanArea>(entity =>
            {
                entity.ToTable("POGO_SCAN_AREA");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasColumnName("ACTIVE")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LatMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MAX");

                entity.Property(e => e.LatMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MIN");

                entity.Property(e => e.LonMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MAX");

                entity.Property(e => e.LonMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MIN");

                entity.Property(e => e.MapId)
                    .HasColumnType("int(11)")
                    .HasColumnName("MAP_ID");
            });

            modelBuilder.Entity<PogoSpecialGym>(entity =>
            {
                entity.HasKey(e => new { e.GymId, e.ChatId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("POGO_SPECIAL_GYMS");

                entity.Property(e => e.GymId)
                    .HasColumnType("int(11)")
                    .HasColumnName("GYM_ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.Data)
                    .HasMaxLength(200)
                    .HasColumnName("DATA");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasColumnName("TYPE");
            });

            modelBuilder.Entity<PogoStop>(entity =>
            {
                entity.ToTable("POGO_STOPS");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Latitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LATITUDE");

                entity.Property(e => e.Longitude)
                    .HasPrecision(12, 9)
                    .HasColumnName("LONGITUDE");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<PogoUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_USER");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Active).HasColumnName("ACTIVE");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(200)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.GroupMembers)
                    .HasColumnType("int(11)")
                    .HasColumnName("GROUP_MEMBERS");

                entity.Property(e => e.IngameName)
                    .HasMaxLength(200)
                    .HasColumnName("INGAME_NAME");

                entity.Property(e => e.IngressName)
                    .HasMaxLength(200)
                    .HasColumnName("INGRESS_NAME");

                entity.Property(e => e.LatMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MAX");

                entity.Property(e => e.LatMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LAT_MIN");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasColumnName("LEVEL");

                entity.Property(e => e.LonMax)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MAX");

                entity.Property(e => e.LonMin)
                    .HasPrecision(12, 9)
                    .HasColumnName("LON_MIN");

                entity.Property(e => e.Team)
                    .HasColumnType("int(11)")
                    .HasColumnName("TEAM");
            });

            modelBuilder.Entity<PogoUserRaid>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("POGO_USER_RAIDS");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("USER_ID");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.GymId)
                    .HasColumnType("int(11)")
                    .HasColumnName("GYM_ID");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasColumnName("LEVEL");

                entity.Property(e => e.PokeForm)
                    .HasMaxLength(1)
                    .HasColumnName("POKE_FORM");

                entity.Property(e => e.PokeId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POKE_ID");

                entity.Property(e => e.RaidId)
                    .HasColumnType("int(11)")
                    .HasColumnName("RAID_ID");

                entity.Property(e => e.Start).HasColumnName("START");

                entity.Property(e => e.TimeMode)
                    .HasColumnType("int(11)")
                    .HasColumnName("TIME_MODE");

                entity.Property(e => e.Title)
                    .HasMaxLength(40)
                    .HasColumnName("TITLE");

                entity.Property(e => e.UpdRaidId)
                    .HasColumnType("int(11)")
                    .HasColumnName("UPD_RAID_ID");
            });

            modelBuilder.Entity<RelChatBot>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.BotId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("REL_CHAT_BOT");

                entity.Property(e => e.ChatId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("CHAT_ID");

                entity.Property(e => e.BotId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("BOT_ID");

                entity.Property(e => e.AllowNotification).HasColumnName("ALLOW_NOTIFICATION");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Level })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("STATES");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasColumnName("LEVEL");

                entity.Property(e => e.Command)
                    .HasMaxLength(50)
                    .HasColumnName("COMMAND");

                entity.Property(e => e.Step)
                    .HasColumnType("int(11)")
                    .HasColumnName("STEP");
            });

            modelBuilder.Entity<UserVote>(entity =>
            {
                entity.HasKey(e => new { e.PollId, e.UserId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("USER_VOTES");

                entity.Property(e => e.PollId)
                    .HasColumnType("int(11)")
                    .HasColumnName("POLL_ID");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Attendee)
                    .HasColumnType("int(11)")
                    .HasColumnName("ATTENDEE");

                entity.Property(e => e.Comment)
                    .HasColumnType("int(11)")
                    .HasColumnName("COMMENT");

                entity.Property(e => e.Time)
                    .HasMaxLength(10)
                    .HasColumnName("TIME");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
