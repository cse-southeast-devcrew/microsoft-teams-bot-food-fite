namespace FoodFite.Models
{
    using Newtonsoft.Json;

    public class UserProfile : IStateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ChannelId { get; set; }
        public int Stains { get; set; }
        public BotProfile BotProfile { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class BotProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ChannelId { get; set; }
    }
}
