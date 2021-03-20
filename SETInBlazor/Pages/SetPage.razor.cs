using AutoMapper;
using Microsoft.AspNetCore.Components;
using Set.Backend.Interfaces;
using Set.Backend.Models;
using Set.Frontend.Constants;
using Set.Frontend.Interfaces;
using Set.Frontend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Set.Frontend.Pages
{
    public partial class SetPage
    {
        [Inject]
        public ICardHelperService _cardHelperService { get; set; }
        [Inject]
        public IUiHelperService _uiHelperService { get; set; }
        [Inject]
        public IMapper _mapper { get; set; }
        [Parameter]
        public string difficultyVariation { get; set; }

        private List<SetCardUiModel> uniqueCardCombinations;
        private int numberOfCardsSelected = 0;
        private int numberOfCardsVisible;
        private GameSettings settings;

        private string lineClass;

        protected override void OnInitialized()
        {
            SetNewGameVariables();

            EnsureSetExistsOnField();

            SetCssVariables();
        }

        private async Task ProcessSelection(SetCardUiModel setCard)
        {
            numberOfCardsSelected = _uiHelperService.GetNumberOfSelectedAfterCardToggle(setCard, numberOfCardsSelected);

            if (IsSetSubmission())
            {
                SignalUiSetSubmissionOutcome();

                await Task.Delay(500);

                ProcessSetReplacement();
            };
        }

        private void ProcessSetReplacement()
        {
            if (IsSetSubmission())
            {
                if (SetIsCorrect())
                {
                    ReplaceSetSubmissionCards();
                    AdjustNumberOfVisibleCards();
                    EnsureSetExistsOnField();
                    SetCssVariables();
                }
                else
                {
                    ResetCssForFailedSetSubmissionCards();
                }
            };
        }

        private bool SetIsCorrect()
        {
            return uniqueCardCombinations.Count(card => card.BackGroundColor == CardBackgroundColor.Success) == 3;
        }

        private void ResetCssForFailedSetSubmissionCards()
        {
            var selectedCards = uniqueCardCombinations.Where(card => card.BackGroundColor == CardBackgroundColor.Failure);

            foreach (var card in selectedCards)
            {
                card.BackGroundColor = CardBackgroundColor.Standard;
                card.Animation = string.Empty;
            }

            numberOfCardsSelected = 0;
        }

        private void SignalUiSetSubmissionOutcome()
        {
            var setSubmission = uniqueCardCombinations.Where(card => card.BackGroundColor == CardBackgroundColor.Selected).ToList();

            var potentialSet = _mapper.Map<List<SetCardUiModel>, List<SetCard>>(setSubmission);
            var isSet = _cardHelperService.VerifySet(potentialSet);

            _uiHelperService.ChangeSetBackgroundColorOnSubmissionOutcome(setSubmission, isSet);
        }

        private List<SetCardUiModel> GetCardsForNewGame(GameSettings settings)
        {
            return _mapper.Map<List<SetCard>, List<SetCardUiModel>>(_cardHelperService.CreateAllUniqueCombinations(settings));
        }

        private bool IsSetSubmission()
        {
            return numberOfCardsSelected == 3;
        }

        private void ReplaceSetSubmissionCards()
        {
            uniqueCardCombinations.RemoveAll(card => card.BackGroundColor == CardBackgroundColor.Success);
            numberOfCardsSelected = 0;
        }

        private void AdjustNumberOfVisibleCards()
        {
            numberOfCardsVisible = NoMoreExtraCardsLeft() ? uniqueCardCombinations.Count : settings.numberOfCardsVisible;
        }

        private void EnsureSetExistsOnField()
        {
            // Get the total number of cards which will be shown and check if they contain a set
            // If not, then add 3 more cards to be visible to ensure there is a set
            var visibleCards = _mapper.Map<List<SetCardUiModel>, List<SetCard>>(uniqueCardCombinations.Take(numberOfCardsVisible).ToList());

            if (!_cardHelperService.DoesFieldContainASet(visibleCards))
            {
                if (NoMoreExtraCardsLeft())
                {
                    SetNewGameVariables();
                }
                else
                {
                    numberOfCardsVisible += 3;
                    EnsureSetExistsOnField();
                }
            }
        }

        private bool NoMoreExtraCardsLeft()
        {
            return uniqueCardCombinations.Count <= numberOfCardsVisible;
        }

        private void SetNewGameVariables()
        {
            difficultyVariation = difficultyVariation ?? "NORMAL";
            settings = new GameSettings(difficultyVariation);
            numberOfCardsVisible = settings.numberOfCardsVisible;

            uniqueCardCombinations = GetCardsForNewGame(settings);
        }

        private void SetCssVariables()
        {
            lineClass = _uiHelperService.GetLineClass(numberOfCardsVisible);
        }
    }
}
