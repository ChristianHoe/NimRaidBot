using EventBot.Business.Commands;
using EventBot.Business.Commands.Raid;
using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.Business.Intrastructure;
using EventBot.Business.Tasks;
using EventBot.Scheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using System.Collections.Generic;

namespace EventBot
{
    public class Startup
    {
        private Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            // Set to false. This will be the default in v5.x and going forward.
            container.Options.ResolveUnregisteredConcreteTypes = false;

            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMemoryCache();

            services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider => (SchedulerHostedService)container.GetInstance<IHostedService>());
            
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(container, options =>
            {  
                options.Container.Options.DefaultLifestyle = SimpleInjector.Lifestyle.Singleton;


                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore()

                    // Ensure activation of a specific framework type to be created by
                    // Simple Injector instead of the built-in configuration system.
                    // All calls are optional. You can enable what you need. For instance,
                    // PageModels and TagHelpers are not needed when you build a Web API.
                    // .AddControllerActivation()
                    // .AddViewComponentActivation()
                    // .AddPageModelActivation()
                    // .AddTagHelperActivation()
                    ;

                // Optionally, allow application components to depend on the non-generic
                // ILogger (Microsoft.Extensions.Logging) or IStringLocalizer
                // (Microsoft.Extensions.Localization) abstractions.
                // options.AddLogging();
                // options.AddLocalization();
            });
            
            
            
            IntegrateSimpleInjector(services);

    //        services.AddDbContext<DataAccess.Models.telegramEntities>(options =>
    //options.UseMySql(Configuration.GetConnectionString("Database")));
        }


        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            // container.Options.DefaultLifestyle = SimpleInjector.Lifestyle.Singleton;
            //container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            InitializeContainer(Configuration, container);

            //services.AddSingleton<IControllerActivator>(
            //    new SimpleInjectorControllerActivator(container));
            //services.AddSingleton<IViewComponentActivator>(
            //    new SimpleInjectorViewComponentActivator(container));

            // services.AddSimpleInjector(container, x => x.Container.Options.DefaultLifestyle = SimpleInjector.Lifestyle.Singleton);
            // services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSimpleInjector(container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

        //    app.UseStaticFiles(new StaticFileOptions()
        //    {
        //        FileProvider = new PhysicalFileProvider(
        //Path.Combine(Directory.GetCurrentDirectory(), @"", "Bilder")),
        //        RequestPath = new PathString("/MyImages")
        //    });


        //    app.UseDirectoryBrowser(new DirectoryBrowserOptions()
        //    {
        //        FileProvider = new PhysicalFileProvider(
        //Path.Combine(Directory.GetCurrentDirectory(), @"", "Bilder")),
        //        RequestPath = new PathString("/MyImages")
        //    });
            // app.UseMvc();


            container.Verify();
        }

        public static void InitializeContainer(IConfiguration Configuration, Container container)
        {
            // Add application presentation components:
            //container.RegisterMvcControllers(app);
            //container.RegisterMvcViewComponents(app);

            // Add application services. For instance:

            var NimRaidBotKey = Configuration.GetValue<string>("TelegramKeys:NimRaidBot");
            container.Register(() => new Business.TelegramProxies.NimRaidBot(NimRaidBotKey));

            var NimPokeBotKey = Configuration.GetValue<string>("TelegramKeys:NimPokeBot");
            container.Register(() => new Business.TelegramProxies.PogoTelegramProxy(NimPokeBotKey));

            var MinunBotKey = Configuration.GetValue<string>("TelegramKeys:MinunBot");
            container.Register(() => new Business.TelegramProxies.MinunBot(MinunBotKey));

            var PlusleBotKey = Configuration.GetValue<string>("TelegramKeys:PlusleBot");
            container.Register(() => new Business.TelegramProxies.PlusleBot(PlusleBotKey));

            var NimFarmBotKey = Configuration.GetValue<string>("TelegramKeys:NimFarmBot");
            container.Register(() => new Business.TelegramProxies.NimFarmBot(NimFarmBotKey));

            var EventBotKey = Configuration.GetValue<string>("TelegramKeys:EventBot");
            container.Register(() => new Business.TelegramProxies.TelegramProxy(EventBotKey));

            var connectionString = Configuration.GetConnectionString("Database");
            container.Register(() => new DataAccess.Database.DatabaseFactory(connectionString));

            var importer3MapId = Configuration.GetValue<int>("MapConfigurations:Importer3:MapId");
            var importer3Url = Configuration.GetValue<string>("MapConfigurations:Importer3:Url");
            var importer3UrlParameter = Configuration.GetValue<string>("MapConfigurations:Importer3:UrlParameter");
            var importer3Name = Configuration.GetValue<string>("MapConfigurations:Importer3:Name");
            var importer3Credentials = Configuration.GetValue<string>("MapConfigurations:Importer3:Credentials");

            container.Register(() => new EventBot.Business.Tasks.NimGoMapBot.Configurations.Importer3(importer3MapId, importer3Url, importer3UrlParameter, importer3Name, importer3Credentials ));


            // var Importer3Credentials = Configuration.GetValue<string>("MapCredentials:Map2");
            // container.Register(() => new Business.Tasks.NimGoMapBot.MapCredentials(Importer3Credentials));


            var operatorTelegramId = Configuration.GetValue<int>("Operator:TelegramId");
            Operator.TelegramId = operatorTelegramId;
            
            // WINDOWS
#if DEBUG
            container.Collection.Register<IScheduledTask>(new[] {
                // typeof( EventBot.Business.Tasks.NimGoMapBot.RocketMapImporter<EventBot.Business.Tasks.NimGoMapBot.Configurations.Importer3>),

                //typeof(Business.NimGoMapBot.Importer3)
                 typeof(Business.NimRaidBot.MessageProcessor)
                , typeof(Business.Tasks.MinunBot.MessageProcessor)
                //typeof(Business.Tasks.NimPokeBot.QuestAnnouncer)
                //,
                //typeof(Business.Tasks.NimFarmBot.MetaCleaner)
                , typeof(Business.NimRaidBot.Announcer)
                //typeof(Business.Tasks.NimRaidBot.NimRaidBotInactiveUserRemover)
                });
#endif


            // LINUX
#if !DEBUG
            container.Collection.Register<IScheduledTask>(new [] {
                //typeof(Business.Tasks.MinunBot.Announcer)
                 typeof(Business.Tasks.MinunBot.MessageProcessor)
                , typeof(Business.Tasks.MinunBot.MinumBotGameAnnouncer)

                // , typeof( EventBot.Business.Tasks.NimGoMapBot.RocketMapImporter<EventBot.Business.Tasks.NimGoMapBot.Configurations.Importer3>) // SGL

                //, typeof(Business.NimPokeBot.Announcer)
                , typeof(Business.NimPokeBot.Announcer2)
                , typeof(Business.NimPokeBot.NimPokeBotCleaner)
                , typeof(Business.NimRaidBot.Announcer)
                , typeof(Business.NimRaidBot.MessageProcessor)
                , typeof(Business.NimRaidBot.PollRemover)
                //, typeof(Business.Tasks.NimRaidBot.NimRaidBotInactiveUserRemover)
                , typeof(Business.Tasks.NimFarmBot.MetaCleaner)
                , typeof(Business.Tasks.NimPokeBot.QuestAnnouncer)
                });
#endif

            // TODO: Exceptionhandling ist doof
            container.Register<IHostedService, SchedulerHostedService>();
            //container.Register<IHostedService>(() => {
            //    var instance = container.GetInstance<SchedulerHostedService>();
            //    instance.UnobservedTaskException += (sender, args) =>
            //    {
            //        Operator.SendMessage(proxy, "Scheduler - ", args.Exception).Wait();
            //        args.SetObserved();
            //    };
            //    return instance;
            //});

            container.Register<PoGoDispatcher, PoGoDispatcher>(SimpleInjector.Lifestyle.Singleton);
            container.Register<RaidDispatcher, RaidDispatcher>(SimpleInjector.Lifestyle.Singleton);
            container.Register<MinunDispatcher, MinunDispatcher>(SimpleInjector.Lifestyle.Singleton);

            container.Register<MemberAdded, MemberAdded>();
            container.Register<MembersAdded, MembersAdded>();
            container.Register<MemberRemoved, MemberRemoved>();

            container.Register<Business.Commands.IHelpCommand, Business.Commands.HelpCommand>(SimpleInjector.Lifestyle.Transient);
            container.Register<Business.Commands.ICancelCommand, Business.Commands.CancelCommand>();

            container.Register<Business.Commands.PoGo.IAnCommand, Business.Commands.PoGo.AnCommand>();
            container.Register<Business.Commands.PoGo.IAusCommand, Business.Commands.PoGo.AusCommand>();
            //container.Register<Business.Commands.PoGo.ISetRaidLevelCommand, Business.Commands.PoGo.SetRaidLevelCommand>();
            container.Register<Business.Commands.PoGo.IUmkreisCommand, Business.Commands.PoGo.UmkreisCommand>();
            container.Register<Business.Commands.PoGo.IPollCommand, Business.Commands.PoGo.PollCommand>();
            container.Register<Business.Commands.PoGo.IRaidPollAnswer, Business.Commands.PoGo.RaidPollAnswer>();

            container.Register<Business.Commands.Raid.IStartCommand, Business.Commands.Raid.StartCommand>();
            // container.Register<Business.Commands.Raid.IConfigureUserCommand, Business.Commands.Raid.ConfigureUserCommand>();
            container.Register<Business.Commands.Raid.ISettingsCommand, Business.Commands.Raid.SettingsCommand>();
            container.Register<Business.Commands.Raid.ICreatePollText, Business.Commands.Raid.CreatePollText>();
            container.Register<Business.Commands.Raid.ICreatePollCommand, Business.Commands.Raid.CreatePollCommand>();
            container.Register<Business.Commands.Raid.IPollAnswer, Business.Commands.Raid.PollAnswer>();
            container.Register<Business.Commands.Raid.IUserAdd, Business.Commands.Raid.UserAdd>();
            container.Register<Business.Commands.Raid.IUserRemove, Business.Commands.Raid.UserRemove>();
            container.Register<Business.Commands.Raid.IPokeCommand, Business.Commands.Raid.PokeCommand>();
            container.Register<Business.Commands.Raid.IIvCommand, Business.Commands.Raid.IvCommand>();
            container.Register<Business.Commands.Raid.IGymsCommand, Business.Commands.Raid.GymsCommand>();
            container.Register<Business.Commands.Raid.IOwnerCommand, Business.Commands.Raid.OwnerCommand>();
            container.Register<Business.Commands.Raid.IUpdateRaidBossCommand, Business.Commands.Raid.UpdateRaidBossCommand>();
            
            container.Register<Business.Commands.Minun.IStartCommand, Business.Commands.Minun.StartCommand>();
            container.Register<Business.Commands.Minun.INutzerCommand, Business.Commands.Minun.NutzerCommand>();
            container.Register<Business.Commands.Minun.IAnCommand, Business.Commands.Minun.AnCommand>();
            container.Register<Business.Commands.Minun.IAusCommand, Business.Commands.Minun.AusCommand>();
            container.Register<Business.Commands.Minun.IRaidBossCommand, Business.Commands.Minun.RaidBossCommand>();
            container.Register<Business.Commands.Minun.IPokeCommand, Business.Commands.Minun.PokeCommand>();
            container.Register<Business.Commands.Minun.ICreateRaidCommand, Business.Commands.Minun.CreateRaidCommand>();
            container.Register<Business.Commands.Minun.IModifyRaidCommand, Business.Commands.Minun.ModifyRaidCommand>();
            container.Register<Business.Commands.Minun.ICreateEventCommand, Business.Commands.Minun.CreateEventCommand>();
            container.Register<Business.Commands.Minun.ISpielCommand, Business.Commands.Minun.SpielCommand>();
            container.Register<Business.Commands.Minun.ISpielAnswer, Business.Commands.Minun.SpielAnswer>();
            container.Register<Business.Commands.Minun.IGamePokeCreateText, Business.Commands.Minun.GamePokeCreateText>();
            container.Register<Business.Commands.Minun.IGymsCommand, Business.Commands.Minun.GymsCommand>();

            container.Register<Business.Commands.Farm.ICreatePollCommand, Business.Commands.Farm.CreatePollCommand>();
            container.Register<Business.Commands.Farm.IPollAnswer, Business.Commands.Farm.PollAnswer>();
            container.Register<Business.Commands.Farm.ISetupPollCommand, Business.Commands.Farm.SetupPollCommand>();
            container.Register<Business.Commands.Farm.ICreateEventSetupText, Business.Commands.Farm.CreateEventSetupText>();
            container.Register<Business.Commands.Farm.IEventSetupAnswer, Business.Commands.Farm.EventSetupAnswer>();

            container.Register<Business.Queries.StatePeakQuery, Business.Queries.StatePeakQuery>(SimpleInjector.Lifestyle.Singleton);
            container.Register<Business.Queries.IGetCurrentPokesQuery, Business.Queries.GetCurrentPokes>(Lifestyle.Singleton);
            container.Register<Business.Queries.IGetPokeQueueQuery, Business.Queries.GetPokeQueue>(Lifestyle.Singleton);
            container.Register<IQuery<IEnumerable<DataAccess.Models.PogoUser>>, Business.Queries.PoGo.ActiveUsersQuery>();
            container.Register<Business.Queries.IGetThrottleQuery, Business.Queries.GetThrottle>(Lifestyle.Singleton);
            container.Register<Business.Queries.IGetCurrentQuestsQuery, Business.Queries.GetCurrentQuests>(Lifestyle.Singleton);
            container.Register<Business.Queries.IGetPogoConfigurationQuery, Business.Queries.GetPogoConfiguration>(Lifestyle.Singleton);
            
            container.Register<DataAccess.Commands.IStatePopCommand, DataAccess.Commands.StatePop>();
            container.Register<DataAccess.Commands.IStatePushCommand, DataAccess.Commands.StatePush>();
            container.Register<DataAccess.Commands.IStateClearCommand, DataAccess.Commands.StateClear>();
            container.Register<DataAccess.Commands.IStateUpdateCommand, DataAccess.Commands.StateUpdate>();

            container.Register<DataAccess.Commands.PoGo.IAnCommand, DataAccess.Commands.PoGo.An>();
            container.Register<DataAccess.Commands.PoGo.IAusCommand, DataAccess.Commands.PoGo.Aus>();
            //container.Register<DataAccess.Commands.PoGo.IRaidLevelCommand, DataAccess.Commands.PoGo.RaidLevel>();
            container.Register<DataAccess.Commands.PoGo.INewPollCommand, DataAccess.Commands.PoGo.NewPoll>();
            container.Register<DataAccess.Commands.PoGo.IPollVoteUpdateCommand, DataAccess.Commands.PoGo.PollVoteUpdate>();

            container.Register<DataAccess.Commands.Raid.IActivateChatCommand, DataAccess.Commands.Raid.ActivateChat>();
            container.Register<DataAccess.Commands.Raid.IAddChatCommand, DataAccess.Commands.Raid.AddChat>();
            container.Register<DataAccess.Commands.Raid.IAddUserCommand, DataAccess.Commands.Raid.AddUser>();
            container.Register<DataAccess.Commands.Raid.ISetEastCommand, DataAccess.Commands.Raid.SetEastCommand>();
            container.Register<DataAccess.Commands.Raid.ISetNordCommand, DataAccess.Commands.Raid.SetNordCommand>();
            container.Register<DataAccess.Commands.Raid.ISetSouthCommand, DataAccess.Commands.Raid.SetSouthCommand>();
            container.Register<DataAccess.Commands.Raid.ISetWestCommand, DataAccess.Commands.Raid.SetWestCommand>();
            container.Register<DataAccess.Commands.Raid.ISetMinRaidLevelCommand, DataAccess.Commands.Raid.SetMinRaidLevelCommand>();

            container.Register<DataAccess.Commands.Raid.IAddRocketMapGymsCommand, DataAccess.Commands.Raid.AddRocketMapGymsCommand>();
            //container.Register<DataAccess.Commands.Raid.IAddGoMapGymsCommand, DataAccess.Commands.Raid.AddGoMapGymsCommand>();
            container.Register<DataAccess.Commands.Raid.IAddRaidsCommand, DataAccess.Commands.Raid.AddRaidsCommand>();
            container.Register<DataAccess.Commands.Raid.IClearRaidsCommand, DataAccess.Commands.Raid.ClearRaidsCommand>();
            container.Register<DataAccess.Commands.Raid.ISetUserLevelCommand, DataAccess.Commands.Raid.SetUserLevelCommand>();
            container.Register<DataAccess.Commands.Raid.ISetUserNameCommand, DataAccess.Commands.Raid.SetUserNameCommand>();
            container.Register<DataAccess.Commands.Raid.ISetUserTeamCommand, DataAccess.Commands.Raid.SetUserTeamCommand>();

            container.Register<DataAccess.Commands.Raid.IDeletePollsByIdsCommand, DataAccess.Commands.Raid.DeletePollsByIdsCommand>();

            container.Register<DataAccess.Commands.Raid.ISetChatForManualRaidAndInitializeCommand, DataAccess.Commands.Raid.SetChatForManualRaidAndInitializeCommand>();
            container.Register<DataAccess.Commands.Raid.ISetGymForManualRaidCommand, DataAccess.Commands.Raid.SetGymForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ISetNowForManualRaidCommand, DataAccess.Commands.Raid.SetNowForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ISetTimeModeForManualRaidCommand, DataAccess.Commands.Raid.SetTimeModeForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ISetRaidLevelForManualRaidCommand, DataAccess.Commands.Raid.SetRaidLevelForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ISetPokeIdForManualRaidCommand, DataAccess.Commands.Raid.SetPokeIdForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ISetTitleForManualRaidCommand, DataAccess.Commands.Raid.SetTitleForManualRaidCommand>();
            container.Register<DataAccess.Commands.Raid.ICreateManuelRaidCommand, DataAccess.Commands.Raid.CreateManuelRaidCommand>();
            container.Register<DataAccess.Commands.Raid.IModifyChatTitleCommand, DataAccess.Commands.Raid.ModifyChatTitle>();
            container.Register<DataAccess.Commands.Raid.ISetIvCommand, DataAccess.Commands.Raid.SetIv>();
            container.Register<DataAccess.Commands.Raid.IUpdateRaidsCommand, DataAccess.Commands.Raid.UpdateRaidsCommand>();
            container.Register<DataAccess.Commands.Raid.IAddSpecialGymCommand, DataAccess.Commands.Raid.AddSpecialGym>();
            container.Register<DataAccess.Commands.Raid.IDeleteSpecialGymCommand, DataAccess.Commands.Raid.DeleteSpecialGym>();
            container.Register<DataAccess.Commands.Raid.ISetRaidIdToUpdateCommand, DataAccess.Commands.Raid.SetRaidIdToUpdateCommand>();
            container.Register<DataAccess.Commands.Raid.ISetPokeIdForRaidCommand, DataAccess.Commands.Raid.SetPokeIdForRaidCommand>();
            container.Register<DataAccess.Commands.Raid.IUpdateMembershipAccessCommand, DataAccess.Commands.Raid.UpdateMembershipAccess>();
            container.Register<DataAccess.Commands.Raid.IRemoveMembershipByUserIdsCommand, DataAccess.Commands.Raid.RemoveMembershipByUserIdsCommand>();
            container.Register<DataAccess.Commands.Raid.IAddRocketMapStopsCommand, DataAccess.Commands.Raid.AddRocketMapStopsCommand>();
            container.Register<DataAccess.Commands.Raid.IAddQuestsCommand, DataAccess.Commands.Raid.AddQuests>();
            container.Register<DataAccess.Commands.Raid.IUpdateQuestsCommand, DataAccess.Commands.Raid.UpdateQuests>();

            container.Register<DataAccess.Commands.Pokes.IAddPokesCommand, DataAccess.Commands.Pokes.AddPokesCommand>();
            //container.Register<DataAccess.Commands.Pokes.IMarkAsReadCommand, DataAccess.Commands.Pokes.MarkAsReadCommand>();
            container.Register<DataAccess.Commands.Pokes.IUpdatePokesCommand, DataAccess.Commands.Pokes.UpdatePokesCommand>();
            container.Register<DataAccess.Commands.Pokes.IRemoveNotificationsByIdsCommand, DataAccess.Commands.Pokes.RemoveNotificationsByIdsCommand>();
            container.Register<DataAccess.Commands.Pokes.IAddPokeNotificationCommand, DataAccess.Commands.Pokes.AddPokeNotificationCommand>();

            container.Register<DataAccess.Commands.Location.IAddNotifyLocationCommand, DataAccess.Commands.Location.AddNotifyLocationCommand>();
            container.Register<DataAccess.Commands.Location.IRemoveNotifyLocationCommand, DataAccess.Commands.Location.RemoveNotifyLocationCommand>();

            container.Register<DataAccess.Commands.Minun.IEnableNotificationsCommand, DataAccess.Commands.Minun.EnableNotifications>();
            container.Register<DataAccess.Commands.Minun.IDisableNotificationsCommand, DataAccess.Commands.Minun.DisableNotifications>();
            //container.Register<DataAccess.Commands.Minun.IMarkPokesAsReadCommand, DataAccess.Commands.Minun.MarkPokesAsReadCommand>();
            container.Register<DataAccess.Commands.Minun.IAddPokeToNotificationListCommand, DataAccess.Commands.Minun.AddPokeToNotificationList>();
            container.Register<DataAccess.Commands.Minun.IRemovePokeFromNotificationListCommand, DataAccess.Commands.Minun.RemovePokeFromNotificationList>();
            container.Register<DataAccess.Commands.Minun.IDeactiveMinunUserCommand, DataAccess.Commands.Minun.DeactiveMinunUser>();
            container.Register<DataAccess.Commands.Minun.IPreferencedBossAddCommand, DataAccess.Commands.Minun.PreferencedBossAddCommand>();
            container.Register<DataAccess.Commands.Minun.IPreferencedBossRemoveCommand, DataAccess.Commands.Minun.PreferencedBossRemoveCommand>();
            container.Register<DataAccess.Commands.Minun.IGameCreateCommand, DataAccess.Commands.Minun.GameCreate>();
            container.Register<DataAccess.Commands.Minun.IGameAnswerCommand, DataAccess.Commands.Minun.GameAnswerCommand>();

            container.Register<DataAccess.Commands.Farm.ICreateEventCommand, DataAccess.Commands.Farm.CreateEvent>();
            container.Register<DataAccess.Commands.Farm.IUpdateEventSetupCommand, DataAccess.Commands.Farm.UpdateEventSetup>();
            container.Register<DataAccess.Commands.Farm.IClearEventMetaCommand, DataAccess.Commands.Farm.ClearEventMetaCommand>();
            container.Register<DataAccess.Commands.Farm.IClearPokeMetaCommand, DataAccess.Commands.Farm.ClearPokeMetaCommand>();
            container.Register<DataAccess.Commands.Farm.IClearPollMetaCommand, DataAccess.Commands.Farm.ClearPollMetaCommand>();
            container.Register<DataAccess.Commands.Farm.IClearRaidMetaCommand, DataAccess.Commands.Farm.ClearRaidMetaCommand>();
            container.Register<DataAccess.Commands.Farm.IClearQuestMetaCommand, DataAccess.Commands.Farm.ClearQuestMetaCommand>();

            container.Register<DataAccess.Queries.IQueryCurrentState, DataAccess.Queries.QueryCurrentState>();
            container.Register<DataAccess.Queries.Base.INumberOfBotsInChatQuery, DataAccess.Queries.Base.NumberOfBotsInChat>();
 
            container.Register<DataAccess.Queries.PoGo.IActiveUsers, DataAccess.Queries.PoGo.ActiveUsers>();
            container.Register<DataAccess.Queries.PoGo.IIsActivePoll, DataAccess.Queries.PoGo.IsActivePoll>();
            container.Register<DataAccess.Queries.PoGo.IActivePoll, DataAccess.Queries.PoGo.ActivePoll>();
            container.Register<DataAccess.Queries.PoGo.IPollVotesUsers, DataAccess.Queries.PoGo.PollVotes>();
            container.Register<DataAccess.Queries.PoGo.IGetAllChatsForArea, DataAccess.Queries.PoGo.GetAllChatsForArea>();
            container.Register<DataAccess.Queries.PoGo.IGetPogoConfiguration, DataAccess.Queries.PoGo.GetPogoConfiguration>();

            container.Register<DataAccess.Queries.Raid.IGetCurrentChatSettingsQuery, DataAccess.Queries.Raid.GetCurrentChatSettings>();
            container.Register<DataAccess.Queries.Raid.IGetCurrentUserSettingsQuery, DataAccess.Queries.Raid.GetCurrentUserSettings>();
            container.Register<DataAccess.Queries.Raid.IGetActiveGymsByChatQuery, DataAccess.Queries.Raid.GetActiveGymsByChat>();
            container.Register<DataAccess.Queries.Raid.IGetGymsQuery, DataAccess.Queries.Raid.GetGyms>();
            container.Register<DataAccess.Queries.Raid.IGetAllRaidsQuery, DataAccess.Queries.Raid.GetAllRaids>();
            // container.Register<DataAccess.Queries.Raid.IGetRaidsQuery, DataAccess.Queries.Raid.GetRaids>();
            container.Register<DataAccess.Queries.Raid.IGetActivePogoGroups, DataAccess.Queries.Raid.GetActivePogoGroups>();
            container.Register<DataAccess.Queries.Raid.IGetRaidByIdQuery, DataAccess.Queries.Raid.GetRaidById>();
            container.Register<DataAccess.Queries.Raid.IGetUserVoteQuery, DataAccess.Queries.Raid.GetUserVote>();
            container.Register<DataAccess.Queries.Raid.IGetActivePollByRaidId, DataAccess.Queries.Raid.GetActivePollByRaidId>();
            container.Register<DataAccess.Queries.Raid.IGetPollsToCleanUpsQuery, DataAccess.Queries.Raid.GetPollsToCleanUp>();
            container.Register<DataAccess.Queries.Raid.IGetActiveChatsForUser, DataAccess.Queries.Raid.GetActiveChatsForUser>();
            container.Register<DataAccess.Queries.Raid.IGetCurrentManualRaidQuery, DataAccess.Queries.Raid.GetCurrentManualRaid>();
            container.Register<DataAccess.Queries.Raid.IGetGymsByChatQuery, DataAccess.Queries.Raid.GetGymsByChat>();
            container.Register<DataAccess.Queries.Raid.IGetNextNewRaidQuery, DataAccess.Queries.Raid.GetNextNewRaid>();
            container.Register<DataAccess.Queries.Raid.IMarkEventAsProcessedQuery, DataAccess.Queries.Raid.MarkEventAsProcessed>();
            container.Register<DataAccess.Queries.Raid.IMarkEventAsProcessingQuery, DataAccess.Queries.Raid.MarkEventAsProcessing>();
            container.Register<DataAccess.Queries.Raid.IGetNextPollToProcessQuery, DataAccess.Queries.Raid.GetNextPollToProcess>();
            container.Register<DataAccess.Queries.Raid.IMarkPollAsProcessedQuery, DataAccess.Queries.Raid.MarkPollAsProcessed>();
            container.Register<DataAccess.Queries.Raid.IMarkPollAsProcessingQuery, DataAccess.Queries.Raid.MarkPollAsProcessing>();
            container.Register<DataAccess.Queries.Raid.IGetSpecialGymsForChatsQuery, DataAccess.Queries.Raid.GetSpecialGymsForChats>();
            container.Register<DataAccess.Queries.Raid.IGetActiveUserRaids, DataAccess.Queries.Raid.GetActiveUserRaids>();
            container.Register<DataAccess.Queries.Raid.IGetUserByIdQuery, DataAccess.Queries.Raid.GetUserById>();
            container.Register<DataAccess.Queries.Raid.IGetActivePollByMessageId, DataAccess.Queries.Raid.GetActivePollByMessageId>();
            container.Register<DataAccess.Queries.Raid.IGetUsersWithoutMinunConfigurationQuery, DataAccess.Queries.Raid.GetUsersWithoutMinunConfiguration>();
            container.Register<DataAccess.Queries.Raid.IGetRaidTimeOffsetsQuery, DataAccess.Queries.Raid.GetRaidTimeOffsets>();
            container.Register<DataAccess.Queries.Raid.IGetInactiveUsersQuery, DataAccess.Queries.Raid.GetInactiveUsers>();

            container.Register<DataAccess.Queries.Raid.IGetStopsQuery, DataAccess.Queries.Raid.GetStops>();
            container.Register<DataAccess.Queries.Raid.IGetAllQuestsQuery, DataAccess.Queries.Raid.GetAllQuests>();
            container.Register<DataAccess.Queries.Raid.IMarkQuestAsProcessedQuery, DataAccess.Queries.Raid.MarkQuestAsProcessed>();
            container.Register<DataAccess.Queries.Raid.IMarkQuestAsProcessingQuery, DataAccess.Queries.Raid.MarkQuestAsProcessing>();
            container.Register<DataAccess.Queries.Raid.IGetNextQuestToProcessQuery, DataAccess.Queries.Raid.GetNextQuestToProcess>();
            container.Register<DataAccess.Queries.Raid.IGetQuestByStopIdQuery, DataAccess.Queries.Raid.GetQuestByStopId>();
            container.Register<DataAccess.Queries.Raid.IGetQuestsQuery, DataAccess.Queries.Raid.GetQuests>();

            //container.Register<DataAccess.Queries.Pokes.IGetModifiedPokesQuery, DataAccess.Queries.Pokes.GetModifiedPokes>();
            container.Register<DataAccess.Queries.Pokes.IGetCurrentPokesQuery, DataAccess.Queries.Pokes.GetCurrentPokes>();
            container.Register<DataAccess.Queries.Pokes.IGetPokesToCleanUpQuery, DataAccess.Queries.Pokes.GetPokesToCleanUp>();
            container.Register<DataAccess.Queries.Pokes.IGetPokesForChatQuery, DataAccess.Queries.Pokes.GetPokesForChat>();
            container.Register<DataAccess.Queries.Pokes.IGetNextNewPokeQuery, DataAccess.Queries.Pokes.GetNextNewPoke>();
            container.Register<DataAccess.Queries.Pokes.IMarkAsProcessingQuery, DataAccess.Queries.Pokes.MarkAsProcessing>();
            container.Register<DataAccess.Queries.Pokes.IMarkAsProcessedQuery, DataAccess.Queries.Pokes.MarkAsProcessed>();
            container.Register<DataAccess.Queries.Pokes.IGetPokeByIdQuery, DataAccess.Queries.Pokes.GetPokeById>();

            container.Register<DataAccess.Queries.Location.IGetCurrentNotificationsQuery, DataAccess.Queries.Location.GetCurrentNotifications>();
            container.Register<DataAccess.Queries.Location.IGetNotifyLocationsByChatIdQuery, DataAccess.Queries.Location.GetNotifyLocationsByChatId>();
            container.Register<DataAccess.Queries.Location.IGetActiveGymsForChatQuery, DataAccess.Queries.Location.GetActiveGymsForChat>();

            container.Register<DataAccess.Queries.Scan.IGetActiveAreas, DataAccess.Queries.Scan.GetActiveAreas>();
            container.Register<DataAccess.Queries.Scan.IGetPokesByAreaId, DataAccess.Queries.Scan.GetPokesByAreaId>();

            container.Register<DataAccess.Queries.Minun.IGetActiveUsers, DataAccess.Queries.Minun.GetActiveUsers>();
            container.Register<DataAccess.Queries.Minun.IGetChatAreas, DataAccess.Queries.Minun.GetChatAreas>();
            container.Register<DataAccess.Queries.Minun.IGetRaidBossPreferencesQuery, DataAccess.Queries.Minun.GetRaidBossPreferences>();
            container.Register<DataAccess.Queries.Minun.IGetRaidBossPreferencesAllQuery, DataAccess.Queries.Minun.GetRaidBossPreferencesAll>();
            container.Register<DataAccess.Queries.Minun.IGetGameAnswersQuery, DataAccess.Queries.Minun.GetGameAnswers>();
            container.Register<DataAccess.Queries.Minun.IGetCurrentGamesQuery, DataAccess.Queries.Minun.GetCurrentGames>();


            container.Register<DataAccess.Queries.Farm.IGetActivePollByEventId, DataAccess.Queries.Farm.GetActivePollByEventId>();
            container.Register<DataAccess.Queries.Farm.IGetActiveGroups, DataAccess.Queries.Farm.GetActiveGroups>();
            container.Register<DataAccess.Queries.Farm.IGetEventsQuery, DataAccess.Queries.Farm.GetEvents>();
            container.Register<DataAccess.Queries.Farm.IGetEventById, DataAccess.Queries.Farm.GetEventById>();
            container.Register<DataAccess.Queries.Farm.IHasDailyFarm, DataAccess.Queries.Farm.HasDailyFarm>();
            container.Register<DataAccess.Queries.Farm.IEventSetupQuery, DataAccess.Queries.Farm.EventSetup>();
            container.Register<DataAccess.Queries.Farm.IUpdatedEventSetupsQuery, DataAccess.Queries.Farm.UpdatedEventSetups>();
            container.Register<DataAccess.Queries.Farm.IEventLocationByIdQuery, DataAccess.Queries.Farm.EventLocationById>();
            container.Register<DataAccess.Queries.Farm.IEventLocationsQuery, DataAccess.Queries.Farm.EventLocations>();
            container.Register<DataAccess.Queries.Farm.IEventLocations2Query, DataAccess.Queries.Farm.EventLocations2>();
            container.Register<DataAccess.Queries.Farm.IIsActiveEventSetupQuery, DataAccess.Queries.Farm.IsActiveEventSetup>();
            container.Register<DataAccess.Queries.Farm.IGetNextEventToProcessQuery, DataAccess.Queries.Farm.GetNextEventToProcess>();
            container.Register<DataAccess.Queries.Farm.IMarkEventAsProcessedQuery, DataAccess.Queries.Farm.MarkEventAsProcessed>();
            container.Register<DataAccess.Queries.Farm.IMarkEventAsProcessingQuery, DataAccess.Queries.Farm.MarkEventAsProcessing>();
            container.Register<DataAccess.Queries.Farm.IGetNextPollToProcessQuery, DataAccess.Queries.Farm.GetNextPollToProcess>();
            container.Register<DataAccess.Queries.Farm.IMarkPollAsProcessedQuery, DataAccess.Queries.Farm.MarkPollAsProcessed>();
            container.Register<DataAccess.Queries.Farm.IMarkPollAsProcessingQuery, DataAccess.Queries.Farm.MarkPollAsProcessing>();

            container.Register<DataAccess.Queries.Base.IGetRocketMapMovesQuery, DataAccess.Queries.Base.GetRocketMapMoves>();
            container.Register<DataAccess.Queries.Base.IGetPokeBaseValuesQuery, DataAccess.Queries.Base.GetPokeBaseValues>();
            container.Register<DataAccess.Queries.Base.IGetPokeNamesQuery, DataAccess.Queries.Base.GetPokeNames>();


            container.Register<DataAccess.Commands.Base.IBotAddCommand, DataAccess.Commands.Base.BotAdd>();
            container.Register<DataAccess.Commands.Base.IBotRemoveCommand, DataAccess.Commands.Base.BotRemove>();
            container.Register<DataAccess.Commands.Base.IUserChannelRelationAddCommand, DataAccess.Commands.Base.UserChannelRelationAdd>();
            container.Register<DataAccess.Commands.Base.IUserChannelRelationRemoveCommand, DataAccess.Commands.Base.UserChannelRelationRemove>();
            container.Register<DataAccess.Commands.Base.IUserChannelRelationRemoveAllCommand, DataAccess.Commands.Base.UserChannelRelationRemoveAll>();


            var registration = container.GetRegistration(typeof(Business.Commands.IHelpCommand)).Registration;

            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.LifestyleMismatch, "Each dispatcher needs its own helper");

            // Cross-wire ASP.NET services (if any). For instance:
            //container.CrossWire<ILoggerFactory>(app);

            // NOTE: Do prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
        }
    }
}
