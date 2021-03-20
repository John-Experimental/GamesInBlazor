using Set.Frontend.Models;
using Set.Frontend.Interfaces;
using System.Collections.Generic;
using Set.Frontend.Constants;

namespace Set.Frontend.Services
{
    public class UiHelperService : IUiHelperService
    {
        public string GetLineClass(int numberOfCardsVisible)
        {
            return numberOfCardsVisible switch
            {
                15 => "five-line",
                12 => "four-line",
                _ => "three-line",
            };
        }

        /// <summary>
        /// Selects or de-selects the card and returns the change in the number of cards selected after the change
        /// </summary>
        /// <param name="setCard">SetCardUiModel to select or de-select</param>
        public int GetNumberOfSelectedAfterCardToggle(SetCardUiModel setCard, int numberOfSelected)
        {
            // If the current background is white, the cards is unselected and thus should be selected otherwise, de-select it. 
            setCard.BackGroundColor = GetBackgroundColorAfterToggle(setCard);

            return RecalculateNumberOfSelectedAfterToggle(setCard, numberOfSelected);
        }

        /// <summary>
        /// Flashes the cards on the board either red or green, depending on if the set was correct
        /// </summary>
        /// <param name="cards">SetCardUiModel cards to flash</param>
        /// /// <param name="isSet">bool to determine if the set was correct or false</param>
        public IEnumerable<SetCardUiModel> ChangeSetBackgroundColorOnSubmissionOutcome(IEnumerable<SetCardUiModel> cards, bool isSet)
        {
            var signalColor = isSet ? CardBackgroundColor.Success : CardBackgroundColor.Failure;

            foreach (var card in cards)
            {
                card.BackGroundColor = signalColor;
                card.Animation = Animations.Pulse;
            }

            return cards;
        }

        private string GetBackgroundColorAfterToggle(SetCardUiModel setCard)
        {
            return setCard.BackGroundColor == CardBackgroundColor.Standard ? CardBackgroundColor.Selected : CardBackgroundColor.Standard;
        }

        private int RecalculateNumberOfSelectedAfterToggle(SetCardUiModel setCard, int numberOfSelected)
        {
            return setCard.BackGroundColor == CardBackgroundColor.Standard ? numberOfSelected - 1 : numberOfSelected + 1;
        }
    }
}
