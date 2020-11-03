namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    public class Cafeteria
    {

        public List<string> _users;
        public Dictionary<string, ConversationReference> _conversation;
        public Dictionary<string, Queue<string>> _actions;
        public Cafeteria() {
            this._users = new List<string>();
            this._conversation = new Dictionary<string, ConversationReference>();
            this._actions = new Dictionary<string, Queue<string>>();
        }

        public void addUser(string name, ConversationReference conversation) {
            _users.Add(name);
            _conversation.Add(name, conversation);
        }
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public string WhenMoneyIsPaid { get; set; }
    }
}