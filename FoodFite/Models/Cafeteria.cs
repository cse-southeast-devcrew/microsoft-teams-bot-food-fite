namespace FoodFite.Models
{
    using System;
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
        public string WhenMoneyIsPaid { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}