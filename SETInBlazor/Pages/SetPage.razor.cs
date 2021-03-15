using AutoMapper;
using Microsoft.AspNetCore.Components;
using Set.Frontend.Models;
using Set.Frontend.Interfaces;
using Set.Backend.Interfaces;
using Set.Backend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

                _uiHelperService.SignalSetSubmissionOutcome(setSubmission, isSet);
            };
        }


        private List<SetCardUiModel> GetCardsForNewGame(GameSettings settings)
        {
            return _mapper.Map<List<SetCard>, List<SetCardUiModel>>(_cardHelperService.CreateAllUniqueCombinations(settings));
        }

        private void ProcessSetReplacement()
        {
            // If it wasn't a set submission, you do nothing
            if (numberOfSelected == 3)
            {
                var redBorderedCards = uniqueCardCombinations.Where(card => card.BorderColor == "red").ToList();
                var countGreenBorders = uniqueCardCombinations.Count(card => card.BorderColor == "green");

                // The while ensures that the 'ProcessSelection' function, which is also called, has run first
                while (redBorderedCards.Count == 0 && countGreenBorders == 0)
                {
                    Thread.Sleep(125);
                    redBorderedCards = uniqueCardCombinations.Where(card => card.BorderColor == "red").ToList();
                    countGreenBorders = uniqueCardCombinations.Count(card => card.BorderColor == "green");
                }

                // Wait 1.5 seconds so that the user can see the set outcome from 'ProcessSelection' before removing it
                Thread.Sleep(1500);

                if (countGreenBorders == 3)
                {
                    // Replace the set by removing the set submission entirely from the list
                    uniqueCardCombinations.RemoveAll(card => card.BackGroundColor == "yellow");
                    numberOfSelected = 0;

                    // Check if the field currently shows more cards than normal (can happen if there was no set previously)
                    // If there are more cards, then remove 3 cards again to bring it back down to 'normal'
                    numberOfCardsVisible -= numberOfCardsVisible > settings.numberOfCardsVisible ? 3 : 0;

                    EnsureSetExistsOnField();
                }
                else
                {
                    foreach (var card in redBorderedCards)
                    {
                        card.BackGroundColor = "white";
                        card.BorderColor = "black";
                    }
                }
            };
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
