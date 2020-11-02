namespace FoodFite.Adapters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Builder.TraceExtensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using FoodFite.Middleware;

    class AdapterWithMiddleWare : BotFrameworkHttpAdapter
    {
        public AdapterWithMiddleWare(IConfiguration configuration, 
            ILogger<BotFrameworkHttpAdapter> logger, 
            CommandMiddleware commandMiddleware, 
            ConversationState conversationState = null)
            : base(configuration, logger)
        {
            if (commandMiddleware == null)
            {
                throw new NullReferenceException(nameof(commandMiddleware));
            }

            Use(commandMiddleware);
        }
    }
}