namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;

    public class Cafeteria
    {
        
        private List<String> _users = new List<String>() {"Bubba", "Kyle", "Kurt"};
            
        public List<string> Users
        {
            get { return _users; }
            set { _users = value; }
        }
        
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public string WhenMoneyIsPaid { get; set; }
    }
}