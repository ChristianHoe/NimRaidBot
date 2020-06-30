using EventBot.Business.TelegramProxies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventBot.Business.Tasks.Discord
{
    public class Test : IScheduledTask
    {
        private readonly PichuProxy _proxy;

        public Test(PichuProxy proxy)
        {
            _proxy = proxy;
        }

        public string Name => "Discord";

        public bool RunExclusive => false;

        public int IntervallInMilliseconds => 1000;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //var x = new PichuProxy();

            _proxy.SendMessage(13.3m, 52.5m, "meins", "Text");
            //_proxy.SendMessage(13.3m, 52.5m, "meins", "Text");
            //int i = 0;

            return Task.CompletedTask;
        }
    }
}
