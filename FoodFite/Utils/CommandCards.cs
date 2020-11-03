namespace FoodFite.Utils
{
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Builder;

    public class CommandCards
    {
        public static IMessageActivity CreateHelpCommandCard()
        {
            var helpCard = new HeroCard
            {
                Title = "Available Commands:",
                Buttons = new List<CardAction>
                {
                    new CardAction { Title = "Create Cafeteria", Type = ActionTypes.ImBack, Value = "create cafeteria" }
                },
            };

            return MessageFactory.Attachment(helpCard.ToAttachment());
        }
    }
}