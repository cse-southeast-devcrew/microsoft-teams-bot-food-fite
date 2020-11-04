namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    public class Cafeteria
    {

        public Dictionary<string, UserProfile> _users;
        public Dictionary<string, ConversationReference> _conversation;
        public Dictionary<string, Queue<string>> _actions;
        public Cafeteria() {
            this._users = new Dictionary<string, UserProfile>();
            this._conversation = new Dictionary<string, ConversationReference>();
            this._actions = new Dictionary<string, Queue<string>>();
        }

        public void addUser(UserProfile user, ConversationReference conversation) {
            _users.Add(user.Name, user);
            _conversation.Add(user.Name, conversation);
        }

        public UserProfile GetUser(string username){
            return _users[username];
        }
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public string WhenMoneyIsPaid { get; set; }
    }
}