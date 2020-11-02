using System.Threading;
using System.Threading.Tasks;
using FoodFite.Commanding;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace FoodFite.Middleware
{
    public class CommandMiddleware : IMiddleware
    {
        public IConfiguration Configuration { get; protected set; }
        public CommandHandler CommandHandler { get; protected set; }

        public CommandMiddleware(IConfiguration configuration)
        {
            Configuration = configuration;
            CommandHandler = new CommandHandler();
        }
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type is ActivityTypes.Message)
            {
                if (await CommandHandler.HandleCommandAsync(turnContext) == false)
                {
                    // TODO: Handle non command
                }
            }
            else
            {
                // Pass along to other Activity type handlers like OnMembersAddedAsync in the Bot
                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}