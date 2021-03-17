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
            SetInitialGameVariables();

            EnsureSetExistsOnField();

            SetCssVariables();
        }

        private async Task ProcessSelection(SetCardUiModel setCard)
        {
            numberOfCardsSelected = _uiHelperService.GetNumberOfSelectedAfterCardToggle(setCard, numberOfCardsSelected);

            if (IsSetSubmission())
            {
                SignalUiSetSubmissionOutcome();

                await Task.Delay(1000);

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
                    ResetNumberOfVisibleCards();
                    EnsureSetExistsOnField();
                }
                else
                {
                    ResetCssForFailedSetSubmissionCards();
                }
            };
        }

        private bool SetIsCorrect()
        {
            return uniqueCardCombinations.Count(card => card.BorderColor == CardBorderColor.Succes) == 3;
        }

        private void ResetCssForFailedSetSubmissionCards()
        {
            var selectedCards = uniqueCardCombinations.Where(card => card.BorderColor == CardBorderColor.Failure);

            foreach (var card in selectedCards)
            {
                card.BackGroundColor = CardBackgroundColor.Standard;
                card.BorderColor = CardBorderColor.Standard;
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
            uniqueCardCombinations.RemoveAll(card => card.BackGroundColor == CardBackgroundColor.Selected);
            numberOfCardsSelected = 0;
        }

        private void ResetNumberOfVisibleCards()
        {
            // Check if the field currently shows more cards than normal (can happen if there was no set previously)
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
                if (NoMoreCardsLeft())
                {
                    uniqueCardCombinations = GetCardsForNewGame(settings);
                }
                else
                {
                    numberOfCardsVisible += 3;
                    EnsureSetExistsOnField();
                }
            }
        }

        private bool NoMoreCardsLeft()
        {
            return uniqueCardCombinations.Count <= numberOfCardsVisible;
        }

        private void SetInitialGameVariables()
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
