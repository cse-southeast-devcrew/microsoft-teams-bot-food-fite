namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    public class Cafeteria
    {

        public List<string> _users;
        public Dictionary<string, ConversationReference> _conversation;
        public Cafeteria() {
            this._users = new List<string>();
            this._conversation = new Dictionary<string, ConversationReference>();
        }

        public void addUser(string name, ConversationReference conversation) {
            _users.Add(name);
            _conversation.Add(name, conversation);
        }
        public static List<string> Users { get; set;}
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public string WhenMoneyIsPaid { get; set; }
    }
}