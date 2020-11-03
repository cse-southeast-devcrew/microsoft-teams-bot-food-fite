namespace FoodFite.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;
    using FoodFite.Services;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Extensions.Configuration;

    class StartDialog : ComponentDialog
    {
        private readonly UserState _userState;

        public StartDialog(UserState userState, IConfiguration configuration) : base(nameof(StartDialog))
        {
            _userState = userState;

            AddDialog(new CafeteriaSetupDialog(new GameStateProvider(configuration)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStateSetupAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStateSetupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: Could adjust OnMembersAddedAsync in GameBot to ask for command and switch on it here to start a different dialog.
            // commands: Create Cafeteria, Status (shows fights status, profile scores, etc), leaderboard, etc
            return await stepContext.BeginDialogAsync(nameof(CafeteriaSetupDialog), null, cancellationToken);
        }
    }
}