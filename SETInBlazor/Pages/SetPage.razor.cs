using AutoMapper;
using Microsoft.AspNetCore.Components;
using Set.Frontend.Models;
using Set.Frontend.Interfaces;
using Set.Backend.Interfaces;
using Set.Backend.Models;
using System.Collections.Generic;
using System.Linq;

namespace Set.Frontend.Pages
{
    public partial class SetPage
    {
        // Using the inject attributes here, rather than in the SET.razor page itself
        // Prefer to keep all the logic away from the page, including the DI
        [Inject]
        public ICardHelperService _cardHelperService { get; set; }
        [Inject]
        public IUiHelperService _uiHelperService { get; set; }
        [Inject]
        public IMapper _mapper { get; set; }
        [Parameter]
        public string difficultyVariation { get; set; }

        private List<SetCardUiModel> uniqueCardCombinations;
        private string lineClass;
        private int numberOfSelected = 0;
        private int numberOfCardsVisible;
        private GameSettings settings;

        protected override void OnInitialized()
        {
            difficultyVariation = difficultyVariation ?? "NORMAL";
            settings = new GameSettings(difficultyVariation);
            numberOfCardsVisible = settings.numberOfCardsVisible;

            uniqueCardCombinations = GetCardsForNewGame(settings);
            EnsureSetExistsOnField();

            lineClass = _uiHelperService.GetLineClass(numberOfCardsVisible);

        }

        private void ProcessSelection(SetCardUiModel setCard)
        {
            numberOfSelected += _uiHelperService.ProcessCardSelection(setCard);

            if (numberOfSelected == 3)
            {
                var setSubmission = uniqueCardCombinations.Where(card => card.BackGroundColor == "yellow").ToList();
                var potentialSet = _mapper.Map<List<SetCardUiModel>, List<SetCard>>(setSubmission);
                var isSet = _cardHelperService.VerifySet(potentialSet);

                if (isSet)
                {
                    _uiHelperService.SignalSetSubmissionOutcome(setSubmission, true);

                    ProcessSetReplacement();

                    EnsureSetExistsOnField();
                }
            };
        }

        private List<SetCardUiModel> GetCardsForNewGame(GameSettings settings)
        {
            return _mapper.Map<List<SetCard>, List<SetCardUiModel>>(_cardHelperService.CreateAllUniqueCombinations(settings));
        }

        private void ProcessSetReplacement()
        {
            uniqueCardCombinations.RemoveAll(card => card.BackGroundColor == "yellow");
            numberOfSelected = 0;

            // Check if the field currently shows more cards than normal (can happen if there was no set)
            // If there are more cards, then remove 3 cards again to bring it back down to 'normal'
            numberOfCardsVisible -= numberOfCardsVisible > settings.numberOfCardsVisible ? 3 : 0;
        }

        private void EnsureSetExistsOnField()
        {
            // Get the total number of cards which will be shown and check if they contain a set
            // If not, then add 3 more cards to be visible to ensure there is a set
            var visibleCards = _mapper.Map<List<SetCardUiModel>, List<SetCard>>(uniqueCardCombinations.Take(numberOfCardsVisible).ToList());

            if (!_cardHelperService.DoesFieldContainASet(visibleCards))
            {
                // Once there are no more cards left and there's no set, start a new game for now
                if (uniqueCardCombinations.Count <= numberOfCardsVisible)
                {
                    uniqueCardCombinations = GetCardsForNewGame(settings);
                }
                else
                {
                    // Otherwise add 3 more cards to be made visible on the field
                    numberOfCardsVisible += 3;
                }
            }
        }
    }
}
