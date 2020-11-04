
namespace FoodFite.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FoodFite.Models;
    using FoodFite.Services;
    using FoodFite.Utils;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class EnterCafeteriaDialog : ComponentDialog
    {
        private readonly StateProvider<Cafeteria> _cafeteriaStateProvider;
        private readonly StateProvider<UserProfile> _userStateProvider;

        public EnterCafeteriaDialog(StateProvider<Cafeteria> cafeteriaStateProvider, StateProvider<UserProfile> userStateProvider) : base(nameof(EnterCafeteriaDialog))
        {
            _cafeteriaStateProvider = cafeteriaStateProvider;
            _userStateProvider = userStateProvider;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                EnterCafeteriaAsync,
                AckAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> EnterCafeteriaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cafeterias = await _cafeteriaStateProvider.ReadAllAsync();

            StringBuilder sb = new StringBuilder();
            sb.Append("Where do you want to eat?");
            cafeterias.ForEach(c => { sb.Append($"\n\n* {c.Name} - Player Count {c.Players.Count}"); });
            
            var prompt = new PromptOptions { Prompt = MessageFactory.Text(sb.ToString()) };
            return await stepContext.PromptAsync(nameof(TextPrompt), prompt, cancellationToken);
        }

        private async Task<DialogTurnResult> AckAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = (string)stepContext.Result;
            Cafeteria cafeteria = await _cafeteriaStateProvider.ReadByIdAsync(result);
            UserProfile user = await _userStateProvider.ReadByIdAsync(stepContext.Context.Activity.From.Id);

            if (cafeteria.Id == user.CafeteriaId)
            {
                await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"You are already in {result} cafeteria"),
                cancellationToken);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(user.CafeteriaId))
                {
                    // TODO: Cosmos isn't transactional and this should be...
                    Cafeteria existingCafeteria = await _cafeteriaStateProvider.ReadByIdAsync(user.CafeteriaId);
                    existingCafeteria.Players.Remove(user);
                    await _cafeteriaStateProvider.UpsertAsync(existingCafeteria);
                }
            }

            cafeteria.Players.Add(user);
            user.CafeteriaId = cafeteria.Id;

            await _cafeteriaStateProvider.UpsertAsync(cafeteria);
            await _userStateProvider.UpsertAsync(user);

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"You are set at {result} cafeteria"),
                cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}