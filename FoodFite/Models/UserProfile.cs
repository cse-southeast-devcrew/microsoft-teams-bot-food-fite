namespace FoodFite.Models
{
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;

    public class UserProfile : IStateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Stains { get; set; }
        public string CafeteriaId { get; set; }
        public ConversationReference ConversationReference { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
