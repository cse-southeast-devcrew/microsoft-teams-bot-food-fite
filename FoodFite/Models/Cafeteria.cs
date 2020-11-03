namespace FoodFite.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Schema;
    public class Cafeteria
    {

        public List<UserProfile> _users;
        public Dictionary<string, ConversationReference> _conversation;
        public Cafeteria() {
            this._users = new List<UserProfile>();
            this._conversation = new Dictionary<string, ConversationReference>();
        }

        public void addUser(string name, ConversationReference conversation) {
            _users.Add(new UserProfile(){
                Name = name,
                Health = 100
            });
            _conversation.Add(name, conversation);
        }

        public UserProfile GetUser(string username){
            return _users.Where(x => x.Name == username).FirstOrDefault<UserProfile>();
        }
        public string Name { get; set; }
        public int MaxNumberOfThrowsInAFite { get; set; }
        public decimal AmountOfDailyLunchMoney { get; set; }
        public string WhenMoneyIsPaid { get; set; }
    }
}