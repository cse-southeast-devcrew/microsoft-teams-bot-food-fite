
namespace FoodFite.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FoodFite.Models;
    using FoodFite.Services;
    using FoodFite.Utils;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class FiteDialog : ComponentDialog
    {
        private readonly StateProvider<UserProfile> _stateProvider;

        public FiteDialog(StateProvider<UserProfile> stateProvider) : base(nameof(FiteDialog))
        {
            _stateProvider = stateProvider;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                StepOneAsync,
                StepTwoAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> StepOneAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: Add rules engine to calculate and control game play.
            UserProfile userProfile = await _stateProvider.ReadByIdAsync(stepContext.Context.Activity.From.Id);
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Your stats are: {userProfile.Stains}"),
                cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> StepTwoAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: Add rules engine to calculate and control game play.
            UserProfile userProfile = await _stateProvider.ReadByIdAsync(stepContext.Context.Activity.From.Id);
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Your stats are: {userProfile.Stains}"),
                cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}