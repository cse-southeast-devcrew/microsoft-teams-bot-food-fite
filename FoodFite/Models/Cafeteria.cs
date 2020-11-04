namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Cafeteria : IStateModel
    {
        [JsonProperty("id")]
        public string Id
        {
            get { return Name; }
        }
        
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public DateTime WhenMoneyIsPaid { get; set; }
        public List<UserProfile> Players { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}