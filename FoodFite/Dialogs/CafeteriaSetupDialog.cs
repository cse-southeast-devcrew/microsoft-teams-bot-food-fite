namespace FoodFite.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Recognizers.Text.DateTime;
    using Microsoft.Recognizers.Text.Number;
    using FoodFite.Models;
    using System;
    using FoodFite.Services;
    using Microsoft.Recognizers.Text;
    using System.Globalization;
    using System.Linq;
    using System.Collections.Generic;

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
            decimal lunchMoney = 1.25m;
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];

            var decimalResults = NumberRecognizer.RecognizeNumber((string)stepContext.Result, Culture.English);
            if (decimalResults.Count > 0)
            {
                foreach (var result in decimalResults)
                {
                    if (result.Resolution.TryGetValue("value", out var value))
                    {
                        lunchMoney = Convert.ToDecimal(value, CultureInfo.InvariantCulture) + 0.00m;
                    }
                }
            }
            
            cafeteria.AmountOfDailyLunchMoney = lunchMoney;

            var prompt = new PromptOptions { Prompt = MessageFactory.Text("What time do you want to pay lunch money? example 12am") };
            return await stepContext.PromptAsync(nameof(TextPrompt), prompt, cancellationToken);
        }

        private async Task<DialogTurnResult> AcknowledgementAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            DateTime when = new DateTime(2020, 11, 3, 0, 0, 0);
            var cafeteria = (Cafeteria)stepContext.Values[CAFETERIA_INFO];

            var dateTimeResults = DateTimeRecognizer.RecognizeDateTime((string)stepContext.Result, Culture.English);
            if (dateTimeResults.Count <= 0 || !dateTimeResults.First().TypeName.StartsWith("datetimeV2"))
            {
                await stepContext.Context.SendActivityAsync("I'm sorry, that doesn't seem to be a valid delivery date and time. Please, try again");
            }

            var first = dateTimeResults.First();
            var resolutionValues = (IList<Dictionary<string, string>>)first.Resolution["values"];
            var subType = first.TypeName.Split('.').Last();

            if (subType.Contains("time") && !subType.Contains("range"))
            {
                when = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();
            }

            cafeteria.WhenMoneyIsPaid = when;

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Thanks for participating!"),
                cancellationToken);

            await _stateProvider.UpsertAsync(cafeteria);
            return await stepContext.EndDialogAsync(stepContext.Values[CAFETERIA_INFO], cancellationToken);
        }
    }
}