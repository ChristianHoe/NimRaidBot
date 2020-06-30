using System;
using System.Globalization;
using System.Net;
using System.Net.Http;

namespace EventBot.Business.TelegramProxies
{

    public class OpenStreetMapAddressDetails
    {
        public int house_number;
        public string road;
        public string suburb;
        public string city_district;
        public string state;
        public string postcode;
        public string country;
        public string country_code;
}

    public class OpenStreetMapAddress
    {
        public long place_id;
        public string licence;
        public string osm_type;
        public long osm_id;
        public decimal lat;
        public decimal lon;
        public int place_rank;
        public string category;
        public string type;
        public double importance;
        public string addresstype;
        public string display_name;
        public string name;

        public OpenStreetMapAddressDetails address;

        public double[] boundingbox;
    }

    public class PichuProxy
    {
        private readonly Discord.Webhook.DiscordWebhookClient _client;
        private readonly HttpClient _webClient;
        private readonly string mapKey;

        public PichuProxy(ulong id, string token, string mapKey)
        {
            this.mapKey = mapKey;
            _client = new Discord.Webhook.DiscordWebhookClient(id, token);
            
            _client.Log += async x => await Console.Out.WriteLineAsync(x.Message);

            // http://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=52.xxx&lon=13.xxx&zoom=18&addressdetails=1
            // https://operations.osmfoundation.org/policies/nominatim/

            _webClient = new HttpClient();
            _webClient.DefaultRequestHeaders.Add("User-Agent", "Discord.PichuBot");
        }
        

        public async void SendMessage(decimal latidute, decimal longitude, string poke, string text)
        {
            var lat = latidute.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            OpenStreetMapAddress addr = null;
            try
            {
                var downloadedAddress = await _webClient.GetStringAsync($"http://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}&zoom=18&addressdetails=1");

                addr = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenStreetMapAddress>(downloadedAddress);
            }
            catch
            { }
            var address = addr == null ? string.Empty : $"{addr.address.road} {addr.address.house_number}";


            var emb = new Discord.EmbedBuilder()
                .WithTitle(poke)
                .WithDescription(text + Environment.NewLine + $"[{address}](https://maps.google.com/?q={lat},{lon})")
                .WithImageUrl($"https://maps.googleapis.com/maps/api/staticmap?center={lat},{lon}&markers=color:red%7C{lat},{lon}&maptype=roadmap&size=175x175&zoom=15&key={mapKey}");

            await _client.SendMessageAsync("", username: "Pichu", embeds: new[] { emb.Build() });
        }
    }
}
