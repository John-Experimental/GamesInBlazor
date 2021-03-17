using Set.Frontend.Models;
using System.Collections.Generic;

namespace Set.Frontend.Interfaces
{
    public interface IUiHelperService
    {
        string GetLineClass(int numberOfCardsVisible);
        int GetNumberOfSelectedAfterCardToggle(SetCardUiModel setCard, int numberOfSelected);
        IEnumerable<SetCardUiModel> ChangeSetBackgroundColorOnSubmissionOutcome(IEnumerable<SetCardUiModel> cards, bool isSet);
    }
}