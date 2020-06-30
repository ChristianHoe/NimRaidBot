using System;
using System.Net.Http;
using System.Text;

namespace EventBot.Business.Tasks.NimGoMapBot.Configurations
{
    public class Importer3 : MapConfiguration
    {
        private string Credentials { get; }

        public Importer3(int mapId, string url, string urlParameter, string name, string credentials) : base(mapId, url, urlParameter, name)
        {
            this.Credentials = credentials;
        }

        public override void SetCredentials(HttpClient request)
        {
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(this.Credentials));

            request.DefaultRequestHeaders.Add("Authorization", "Basic " + svcCredentials);
            request.DefaultRequestHeaders.Add("Referer", this.Url);
        }
    }
}