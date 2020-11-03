// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.10.3

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FoodFite.Bots;
using FoodFite.Utils;
using FoodFite.Dialogs;
using FoodFite.Services;

namespace FoodFite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            // services.AddSingleton<IStorage, MemoryStorage>();
            
            services.AddSingleton<IStorage>(
            new CosmosDbPartitionedStorage(
                new CosmosDbPartitionedStorageOptions
                {
                    CosmosDbEndpoint = Configuration.GetValue<string>(EnvironmentConstants.CosmosEndpoint),
                    AuthKey = Configuration.GetValue<string>(EnvironmentConstants.CosmosPrimaryKey),
                    DatabaseId = Configuration.GetValue<string>(EnvironmentConstants.CosmosDatabaseId),
                    ContainerId = Configuration.GetValue<string>(EnvironmentConstants.CosmosContainerId),
                    CompatibilityMode = false,
                }
                )
            );

            // Create the User state. (Used in this bot's Dialog implementation.) ; DialogBot.cs
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.) ; DialogBot.cs
            services.AddSingleton<ConversationState>();

            var botType = Environment.GetEnvironmentVariable(EnvironmentConstants.BotType);

            switch (botType)
            {
                case EnvironmentConstants.BotTypeValueFiteBot:
                    services.AddTransient<IBot, FightBot>();
                    break;
                case EnvironmentConstants.BotTypeValueGameBot: // Ian's attempt at thinking, probably wrong as usual.
                    services.AddSingleton<StartDialog>();
                    services.AddTransient<IBot, GameBot<StartDialog>>();
                    break;
                default:
                    services.AddTransient<IBot, FightBot>();
                    break;
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
