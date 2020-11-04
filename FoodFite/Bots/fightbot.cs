// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.10.3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using FoodFite.Models;
using FoodFite.Factories;

namespace FoodFite.Bots
{
// This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
public class FightBot : ActivityHandler 
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        private IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly Cafeteria _cafeteria;

        private readonly IBotFrameworkHttpAdapter _adapter;

                // Messages sent to the user.
        private const string WelcomeMessage = "Hack project to build a " +
                                              "multiplayer game inspired " +
                                              "by the Food Fight game from " +
                                              "the days of old";

     
        
        public FightBot(ConversationState conversationState, UserState userState, Cafeteria cafeteria, IBotFrameworkHttpAdapter adapter)
        {
            _conversationState = conversationState;
            _userState = userState;
            _cafeteria = cafeteria;
            _adapter = adapter;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeCard = new HeroCard
                        {
                            Text = WelcomeMessage,
                            Images = new List<CardImage>() { new CardImage("https://foodfiteblobstorage.blob.core.windows.net/pictures/food-fight-blog.jpg") },
                            Buttons = new List<CardAction>()
                            {
                                new CardAction(ActionTypes.ImBack, "Fight!",  value: "Fight!"),
                            }
                        };

                    var welcomeResponse = MessageFactory.Attachment(welcomeCard.ToAttachment());
                    await turnContext.SendActivityAsync(welcomeResponse, cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
           
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationFlow>(nameof(ConversationFlow));
            //turnContext.Activity.GetConversationReference
            var flow = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationFlow(), cancellationToken);

            _userProfileAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var profile = await _userProfileAccessor.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

            await FillOutUserProfileAsync(flow, profile, turnContext, cancellationToken);

            // Save changes.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private async Task FillOutUserProfileAsync(ConversationFlow flow, UserProfile profile, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();
            string message;

            switch (flow.LastQuestionAsked)
            {
                case ConversationFlow.Question.None:
                    await turnContext.SendActivityAsync("Let's get started. What is your name?", null, null, cancellationToken);
                    flow.LastQuestionAsked = ConversationFlow.Question.Name;
                    break;
                case ConversationFlow.Question.Name:
                    if (ValidateName(input, out var name, out message))
                    {
                        profile.Name = name;
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.ChangeClothes((Protection)ItemFactory.WhiteTeeShirtFactory());
                        _cafeteria.addUser(profile.Name, turnContext.Activity.GetConversationReference());
                        await turnContext.SendActivityAsync($"Hi {profile.Name}. You are currently armed with a {profile.Clothes.Name}", null, null, cancellationToken);

                        var buttons = new List<CardAction>();
                        foreach(UserProfile username in _cafeteria._users) {
                            if(username.Name != profile.Name) {
                                var action = new CardAction(ActionTypes.ImBack, username.Name, value: username.Name);
                                buttons.Add(action);
                            }
                        }

                        var userCards = new HeroCard
                        {
                            Title = "Whom do you wish to fight?",
                            Buttons = buttons
                        };

                        var fightresponse = MessageFactory.Attachment(userCards.ToAttachment());
                        await turnContext.SendActivityAsync(fightresponse, cancellationToken);
                        flow.LastQuestionAsked = ConversationFlow.Question.Opponent;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
                case ConversationFlow.Question.Opponent:
                    if (ValidateName(input, out var opponent, out message))
                    {
                        profile.Opponent = _cafeteria.GetUser(opponent);
                        //we need to find a way to not attach the opponent to the profile, prevents multiple fights at once.
                        await turnContext.SendActivityAsync($"I have your opponent as {profile.Opponent.Name}.", null, null, cancellationToken);
                        await turnContext.SendActivityAsync("Attack with?", null, null, cancellationToken);
                        
                        var buttons = new List<CardAction>();
                        foreach( Food item in profile.ListFood()) {
                            var action = new CardAction(ActionTypes.ImBack, $"{item.Name}: {item.Ammo} left", value: item.Name);
                            buttons.Add(action);
                        }

                        var weaponcard = new HeroCard
                        {
                            Title = "Choose your weapon",
                            Buttons = buttons
                        };

                        var weaponresponse = MessageFactory.Attachment(weaponcard.ToAttachment());
                        await turnContext.SendActivityAsync(weaponresponse, cancellationToken);
                        
                        flow.LastQuestionAsked = ConversationFlow.Question.Weapon;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }

                case ConversationFlow.Question.Weapon:
                    if (ValidateName(input, out var weapon, out message))
                    {
                        profile.Weapon = profile.FoodMap[weapon];
                        int damage = (int)(profile.ThrowFood(profile.FoodMap[weapon], profile.Opponent));
                        //we need to find a way to not attach the weapon to the profile, prevents multiple fights at once.
                        await turnContext.SendActivityAsync($"You threw a {profile.Weapon.Name} at {profile.Opponent.Name} and dealt {damage} damage!");
                        
                        var buttons = new List<CardAction>();
                        foreach(UserProfile username in _cafeteria._users) {
                            if(username.Name != profile.Name) {
                                var action = new CardAction(ActionTypes.ImBack, username.Name, value: username.Name);
                                buttons.Add(action);
                            }
                        }

                        var userCards = new HeroCard
                        {
                            Title = "Whom do you wish to fight?",
                            Buttons = buttons
                        };
                        var fightresponse = MessageFactory.Attachment(userCards.ToAttachment());
                        await turnContext.SendActivityAsync(fightresponse, cancellationToken);

                        Queue<string> actionQueue;
                        if(!_cafeteria._actions.ContainsKey(profile.Opponent.Name)) {
                            actionQueue = new Queue<string>();
                            _cafeteria._actions.Add(profile.Opponent.Name, actionQueue);
                        } else {
                            actionQueue = _cafeteria._actions[profile.Opponent.Name];
                        }
                        actionQueue.Enqueue($"{profile.Name} hit you with {profile.Weapon.Name} for {damage} damage!");
                        await ((BotAdapter)_adapter).ContinueConversationAsync("asdf", _cafeteria._conversation[profile.Opponent.Name], notifyPlayer , default(CancellationToken));
                        
                        flow.LastQuestionAsked = ConversationFlow.Question.Opponent;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
            }
        }

        private async Task notifyPlayer(ITurnContext context, CancellationToken token) {
            var profile = await _userProfileAccessor.GetAsync(context, () => new UserProfile(), token);
            await context.SendActivityAsync(_cafeteria._actions[profile.Name].Dequeue());
        }

        private static bool ValidateName(string input, out string name, out string message)
        {
            name = null;
            message = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                message = "Please enter a name that contains at least one character.";
            }
            else
            {
                name = input.Trim();
            }

            return message is null;
        }

        private static bool ValidateAge(string input, out int age, out string message)
        {
            age = 0;
            message = null;

            // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
            try
            {
                // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
                // The recognizer returns a list of potential recognition results, if any.

                var results = NumberRecognizer.RecognizeNumber(input, Culture.English);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "value" entry contains the processed string.
                    if (result.Resolution.TryGetValue("value", out var value))
                    {
                        age = Convert.ToInt32(value);
                        if (age >= 18 && age <= 120)
                        {
                            return true;
                        }
                    }
                }

                message = "Please enter an age between 18 and 120.";
            }
            catch
            {
                message = "I'm sorry, I could not interpret that as an age. Please enter an age between 18 and 120.";
            }

            return message is null;
        }

        private static bool ValidateDate(string input, out string date, out string message)
        {
            date = null;
            message = null;

            // Try to recognize the input as a date-time. This works for responses such as "11/14/2018", "9pm", "tomorrow", "Sunday at 5pm", and so on.
            // The recognizer returns a list of potential recognition results, if any.
            try
            {
                var results = DateTimeRecognizer.RecognizeDateTime(input, Culture.English);

                // Check whether any of the recognized date-times are appropriate,
                // and if so, return the first appropriate date-time. We're checking for a value at least an hour in the future.
                var earliest = DateTime.Now.AddHours(1.0);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "values" entry contains the processed input.
                    var resolutions = result.Resolution["values"] as List<Dictionary<string, string>>;

                    foreach (var resolution in resolutions)
                    {
                        // The processed input contains a "value" entry if it is a date-time value, or "start" and
                        // "end" entries if it is a date-time range.
                        if (resolution.TryGetValue("value", out var dateString)
                            || resolution.TryGetValue("start", out dateString))
                        {
                            if (DateTime.TryParse(dateString, out var candidate)
                                && earliest < candidate)
                            {
                                date = candidate.ToShortDateString();
                                return true;
                            }
                        }
                    }
                }

                message = "I'm sorry, please enter a date at least an hour out.";
            }
            catch
            {
                message = "I'm sorry, I could not interpret that as an appropriate date. Please enter a date at least an hour out.";
            }

            return false;
        }
    
        

    }
}