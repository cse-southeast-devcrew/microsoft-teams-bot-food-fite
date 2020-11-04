
namespace FoodFite.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using FoodFite.Models;
    using FoodFite.Services;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    public class StatsDialog : ComponentDialog
    {
        private readonly StateProvider<UserProfile> _stateProvider;

        public StatsDialog(StateProvider<UserProfile> stateProvider) : base(nameof(StatsDialog))
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
            UserProfile userProfile = await _stateProvider.ReadByIdAsync(stepContext.Context.Activity.From.Id);
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Your stats are: {userProfile.Stains}"),
                cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}