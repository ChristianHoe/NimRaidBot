using System.Net.Http;

namespace EventBot.Business.Tasks.NimGoMapBot.Configurations
{
    public abstract class MapConfiguration
    {
        public int MAP_ID { get; }
        public string Url { get; }
        public string UrlParameter { get; }
        public string Name { get; }

        public virtual bool RunExclusive => false;
        public virtual int IntervallInMilliseconds { get { return 10 * 1000; } }

        public abstract void SetCredentials(HttpClient request);

        public MapConfiguration(int mapId, string url, string urlParameter, string name)
        {
            MAP_ID = mapId;
            Url = url;
            UrlParameter = urlParameter;
            Name = name;
        }
    }
}