namespace FoodFite.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using FoodFite.Models;
    using System;
    using FoodFite.Services;

    class CafeteriaSetupDialog : ComponentDialog
    {
        private const string CAFETERIA_INFO = "value-cafeteriaInfo";
        private readonly StateProvider<Cafeteria> _stateProvider;

        public CafeteriaSetupDialog(StateProvider<Cafeteria> stateProvider) : base(nameof(CafeteriaSetupDialog))
        {
            _stateProvider = stateProvider;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                MaxFitesAsync,
                LunchMoneyPerDayAsync,
                WhatTimeToPayAsync,
                AcknowledgementAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var prompt = new PromptOptions { Prompt = MessageFactory.Text("Please enter the cafeteria's name.") };
            return await stepContext.PromptAsync(nameof(TextPrompt), prompt, cancellationToken);
        }

        private static async Task<DialogTurnResult> MaxFitesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[CAFETERIA_INFO] = new Cafeteria();
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];
            cafeteria.Name = (string)stepContext.Result;

            var prompt = new PromptOptions { Prompt = MessageFactory.Text("How many throws in a fite?") };
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), prompt, cancellationToken);
        }

        private static async Task<DialogTurnResult> LunchMoneyPerDayAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];
            cafeteria.MaxNumberOfThrowsInAFite = (int)stepContext.Result;

            var prompt = new PromptOptions { Prompt = MessageFactory.Text("How much lunch money to pay per day?") };
            return await stepContext.PromptAsync(nameof(TextPrompt), prompt, cancellationToken);
        }

        private static async Task<DialogTurnResult> WhatTimeToPayAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];
            // TODO: Need to make NumberPrompt work with Decimal from LunchMoneyPerDayAsync
            decimal result = Convert.ToDecimal(stepContext.Result);
            cafeteria.AmountOfDailyLunchMoney = result;

            var prompt = new PromptOptions { Prompt = MessageFactory.Text("What time do you want to pay lunch money?") };
            return await stepContext.PromptAsync(nameof(TextPrompt), prompt, cancellationToken);
        }

        private async Task<DialogTurnResult> AcknowledgementAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];
            // TODO: DateTime Prompt in WhatTimeToPayAsync and convert to datetime here and model
            cafeteria.WhenMoneyIsPaid = (string)stepContext.Result;

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Thanks for participating!"),
                cancellationToken);

            var result = await _stateProvider.UpsertAsync(cafeteria);

            if (result == null)
                throw new NullReferenceException("Something happened writing to database"); // TODO: do something different here

            // TODO: I believe this returns the value bag to the parent dialog, next in waterfall. I think we could persist this to cosmos
            // using the SDK (not bot state)? Should we use botstate? It's shared accross users so shouldn't be in user state.
            return await stepContext.EndDialogAsync(stepContext.Values[CAFETERIA_INFO], cancellationToken);
        }
    }
}