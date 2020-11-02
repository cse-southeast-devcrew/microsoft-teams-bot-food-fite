using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace FoodFite.Commanding
{
    public class CommandHandler
    {
        public async virtual Task<bool> HandleCommandAsync(ITurnContext context)
        {
            Activity activity = context.Activity;
            Command command = Command.FromMessageActivity(activity);
            Activity replyActivity = null;

            if (command == null) return false;

            bool wasHandled = false;
            
            switch (command.BaseCommand)
            {
                case Commands.CreateCafeteria:
                    // TODO: implement Create Cafeteria
                    wasHandled = true;
                    break;
                
                default:
                    replyActivity = activity.CreateReply("Command not recognized.");
                    break;
            }

            if (replyActivity != null) await context.SendActivityAsync(replyActivity);

            return wasHandled;
        }
    }
}