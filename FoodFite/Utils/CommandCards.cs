namespace FoodFite.Utils
{
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Builder;
    using FoodFite.Models;

    public class CommandCards
    {
        public static IMessageActivity CreateHelpCommandCard()
        {
            var helpCard = new HeroCard
            {
                Title = "Available Commands:",
                Buttons = new List<CardAction>
                {
                    new CardAction { Title = "Create Cafeteria", Type = ActionTypes.ImBack, Value = "create cafeteria" },
                    new CardAction { Title = "Show my Stats", Type = ActionTypes.ImBack, Value = "stats" },
                    new CardAction { Title = "Enter Cafeteria", Type = ActionTypes.ImBack, Value = "enter cafeteria" },
                    new CardAction { Title = "Start a Fite", Type = ActionTypes.ImBack, Value = "fite" },
                    new CardAction { Title = "Show Leaderboard", Type = ActionTypes.ImBack, Value = "leaderboard" },
                },
            };

            return MessageFactory.Attachment(helpCard.ToAttachment());
        }

        public static Attachment CreateEnterCafeteriaCommandCard(List<Cafeteria> cafeterias)
        {
            var helpCard = new HeroCard
            {
                Title = "Choose a Cafeteria:",
                Buttons = new List<CardAction>()
            };

            foreach (Cafeteria cafeteria in cafeterias)
                helpCard.Buttons.Add(new CardAction { Title = cafeteria.Name, Type = ActionTypes.ImBack, Value = cafeteria.Id });

            return helpCard.ToAttachment();
        }
    }
}