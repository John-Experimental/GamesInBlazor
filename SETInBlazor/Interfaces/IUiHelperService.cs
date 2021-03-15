using Set.Frontend.Models;
using System.Collections.Generic;

namespace Set.Frontend.Interfaces
{
    public interface IUiHelperService
    {
        string GetLineClass(int numberOfCardsVisible);
        int ProcessCardSelection(SetCardUiModel setCard);
        IEnumerable<SetCardUiModel> SignalSetSubmissionOutcome(IEnumerable<SetCardUiModel> cards, bool isSet);
    }
}