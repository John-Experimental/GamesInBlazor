using Set.Frontend.Models;
using Set.Frontend.Interfaces;
using System.Collections.Generic;

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
        public int ProcessCardSelection(SetCardUiModel setCard)
        {
            // If the current background is white, the cards is unselected and thus should be selected otherwise, de-select it. 
            setCard.BackGroundColor = setCard.BackGroundColor == "white" ? "yellow" : "white";

            return setCard.BackGroundColor == "white" ? -1 : 1;
        }

        /// <summary>
        /// Flashes the cards on the board either red or green, depending on if the set was correct
        /// </summary>
        /// <param name="cards">SetCardUiModel cards to flash</param>
        /// /// <param name="isSet">bool to determine if the set was correct or false</param>
        public void SignalSetSubmissionOutcome(IEnumerable<SetCardUiModel> cards, bool isSet)
        {
            
            var signalColor = isSet ? "green" : "red";

            for (int i = 0; i < 10; i++)
            {
                foreach (var card in cards)
                {
                    card.BackGroundColor = signalColor;
                    card.BackGroundColor = "white";
                }

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
