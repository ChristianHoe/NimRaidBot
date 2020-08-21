using System;
using System.IO;
using System.Threading.Tasks;
using EventBot;
using EventBot.Business.Commands.Minun;
using EventBot.Business.Helper;
using EventBot.Business.TelegramProxies;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventBot.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("hosting.json", optional: false)
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

            var container = new SimpleInjector.Container();
            container.Options.DefaultLifestyle = SimpleInjector.Lifestyle.Singleton;
            Startup.InitializeContainer(configuration, container);

try
{
            container.Verify();
                        
            var sut2 = container.GetInstance<IPokeCommand>();
            var bot = container.GetInstance<MinunBot>();
            var message = new Telegram.Bot.Types.Message();
            message.Chat = new Telegram.Bot.Types.Chat();
            message.Chat.Id = (long)Operator.TelegramId;
            message.From = new Telegram.Bot.Types.User();
            message.From.Id = (int)Operator.TelegramId;
            
            await sut2.ExecuteAsync(message, "", bot);

            return;
}
catch (Exception ex)
{

}


        }
    }
}
