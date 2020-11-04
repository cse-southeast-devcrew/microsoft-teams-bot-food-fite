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
                    flow.LastQuestionAsked = ConversationFlow.Question.Action;
                    break;

                case ConversationFlow.Question.Action:
                    if (ValidateName(input, out var name, out message))
                    {
                        profile.Name = name;
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.addFood((Food)ItemFactory.RandomFoodFactory());
                        profile.ChangeClothes((Protection)ItemFactory.RandomDefenseGearFactory());
                        profile.Health = 100;
                        _cafeteria.addUser(profile, turnContext.Activity.GetConversationReference());
                        await turnContext.SendActivityAsync($"Hi {profile.Name}. You are currently armed with a {profile.Clothes.Name}", null, null, cancellationToken);

                        await turnContext.SendActivityAsync(ActionQuestion(), cancellationToken);
                        flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }

                case ConversationFlow.Question.Back:
                    await turnContext.SendActivityAsync(ActionQuestion(), cancellationToken);
                    flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                    break;

                case ConversationFlow.Question.ActionRouting:
                    if (ValidateName(input, out var action, out message))
                    {
                        if (_cafeteria._users.ContainsKey(profile.Name))
                        {
                            switch (action.ToLower())
                            {
                                case "throw food":
                                    await turnContext.SendActivityAsync(SelectTargetQuestion(profile), cancellationToken);
                                    flow.LastQuestionAsked = ConversationFlow.Question.Opponent;
                                    break;
                                case "check status":
                                    StateStatus(turnContext, profile, cancellationToken);

                                    await turnContext.SendActivityAsync(ActionQuestion(), cancellationToken);
                                    flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                                    break;
                                default:
                                    await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                                    await turnContext.SendActivityAsync(ActionQuestion(), cancellationToken);
                                    flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                                    break;
                            }
                        }
                        else
                        {
                            await turnContext.SendActivityAsync(message ?? "You're in detention, no actions permitted.");
                            await turnContext.SendActivityAsync(ListRemainingPlayers(), cancellationToken);
                            flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                        }
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
                        profile.Opponent = _cafeteria.GetUser(opponent).Name;
                        //we need to find a way to not attach the opponent to the profile, prevents multiple fights at once.
                        await turnContext.SendActivityAsync($"I have your opponent as {profile.Opponent}.", null, null, cancellationToken);
                        await turnContext.SendActivityAsync("Attack with?", null, null, cancellationToken);
                        await turnContext.SendActivityAsync(SelectWeaponQuestion(profile), cancellationToken);

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
                        int damage = (int)(profile.ThrowFood(profile.FoodMap[weapon]));
                        //we need to find a way to not attach the weapon to the profile, prevents multiple fights at once.
                        await turnContext.SendActivityAsync($"You threw a {profile.Weapon.Name} at {profile.Opponent} and dealt {damage} damage!");

                        Queue<string> actionQueue;
                        if (!_cafeteria._actions.ContainsKey(profile.Opponent))
                        {
                            actionQueue = new Queue<string>();
                            _cafeteria._actions.Add(profile.Opponent, actionQueue);
                        }
                        else
                        {
                            actionQueue = _cafeteria._actions[profile.Opponent];
                        }
                        actionQueue.Enqueue($"{profile.Name},{profile.Weapon.Name},{damage}");
                        await ((BotAdapter)_adapter).ContinueConversationAsync("asdf", _cafeteria._conversation[profile.Opponent], notifyPlayer, default(CancellationToken));

                        await turnContext.SendActivityAsync(ActionQuestion(), cancellationToken);
                        flow.LastQuestionAsked = ConversationFlow.Question.ActionRouting;
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
            }
        }

        private async Task notifyPlayer(ITurnContext context, CancellationToken token)
        {
            var profile = await _userProfileAccessor.GetAsync(context, () => new UserProfile(), token);

            var message = _cafeteria._actions[profile.Name].Dequeue().Split(",");
            var player = message[0];
            var food = message[1];
            var damage = double.Parse(message[2]);
            var taken = (int)(_cafeteria.GetUser(profile.Name).GetHit(damage));

            await context.SendActivityAsync($"{player} threw {food} at you for {damage} damage. You take {taken} damage.");

            if (_cafeteria.GetUser(profile.Name).Health <= 0)
            {
                _cafeteria._users.Remove(profile.Name);
                await context.SendActivityAsync($"Defeat: You've been hit by too much food and now have detention");
            }
        }

        private IMessageActivity ListRemainingPlayers()
        {
            var buttons = new List<CardAction>();
            foreach (string username in _cafeteria._users.Keys)
            {
                var actioncard = new CardAction(ActionTypes.ImBack, "username", value: "");
                buttons.Add(actioncard);
            }

            var userCard = new HeroCard
            {
                Title = "Remaining Players",
                Buttons = buttons
            };
            return MessageFactory.Attachment(userCard.ToAttachment());
        }

        private IMessageActivity ActionQuestion()
        {
            var actionButtons = new List<CardAction>();
            actionButtons.Add(new CardAction(ActionTypes.ImBack, "Throw Food", value: "Throw Food"));
            actionButtons.Add(new CardAction(ActionTypes.ImBack, "Check Status", value: "Check Status"));

            var userCards = new HeroCard
            {
                Title = "What action would you like to do?",
                Buttons = actionButtons
            };

            return MessageFactory.Attachment(userCards.ToAttachment());
        }

        private IMessageActivity SelectTargetQuestion(UserProfile profile)
        {
            var buttons = new List<CardAction>();
            foreach (string username in _cafeteria._users.Keys)
            {
                if (username != profile.Name)
                {
                    var actioncard = new CardAction(ActionTypes.ImBack, username, value: username);
                    buttons.Add(actioncard);
                }
            }

            var userCard = new HeroCard
            {
                Title = "Who is your target?",
                Buttons = buttons
            };

            return MessageFactory.Attachment(userCard.ToAttachment());
        }

        private IMessageActivity SelectWeaponQuestion(UserProfile profile)
        {
            var buttons = new List<CardAction>();
            foreach (var item in profile.FoodMap)
            {
                var action = new CardAction(ActionTypes.ImBack, $"{item.Key}: {item.Value.Ammo} left", value: item.Key);
                buttons.Add(action);
            }

            var weaponcard = new HeroCard
            {
                Title = "Choose your weapon",
                Buttons = buttons
            };

            return MessageFactory.Attachment(weaponcard.ToAttachment());
        }

        private async void StateStatus(ITurnContext turnContext, UserProfile user, CancellationToken cancellationToken)
        {
            var profile = _cafeteria._users[user.Name];
            await turnContext.SendActivityAsync($"You have {(int)(profile.Health)} health remaining.", null, null, cancellationToken);
            if (profile.Clothes != null)
            {
                await turnContext.SendActivityAsync($"Your {profile.Clothes.Name} has {(int)(profile.Clothes.Health)} health remaining.", null, null, cancellationToken);
            }
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
    }
}