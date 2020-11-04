
namespace FoodFite.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using FoodFite.Models;
    using FoodFite.Services;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    public class FiteDialog : ComponentDialog
    {
        private readonly StateProvider<UserProfile> _stateProvider;

        public FiteDialog(StateProvider<UserProfile> stateProvider) : base(nameof(FiteDialog))
        {
            _stateProvider = stateProvider;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetStatsAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GetStatsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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