// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.10.3

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FoodFite.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using FoodFite.Services;
using Microsoft.Extensions.Configuration;

namespace FoodFite.Bots
{
    public class GameBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly IConfiguration _configuration;

        public GameBot(ConversationState conversationState, UserState userState, IConfiguration configuration, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            _configuration = configuration;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = new UserProfile { BotProfile = new BotProfile() };

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    userProfile.Id = member.Id;
                    userProfile.Name = member.Name;
                }
                else
                {
                    userProfile.BotProfile.Id = member.Id;
                    userProfile.BotProfile.Name = member.Name;
                }
            }

            StateProvider<UserProfile> stateProvider = new StateProvider<UserProfile>(_configuration);
            var existingUserProfile = await stateProvider.ReadByIdAsync(userProfile);

            if (existingUserProfile != null)
            {
                var reply = MessageFactory.Text($"Welcome back to the Food Fite Bot {userProfile.Name}. Type a command or \"help\" to get started.");
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                await stateProvider.UpsertAsync(userProfile); // TODO: Handle this better
                var reply = MessageFactory.Text($"Welcome to the Food Fite Bot {userProfile.Name}. Type a command or \"help\" to get started.");
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
        }
    }
}
