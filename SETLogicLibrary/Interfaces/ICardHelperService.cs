using SETLogicLibrary.Models;
using System.Collections.Generic;

namespace SETLogicLibrary.Interfaces
{
    public interface ICardHelperService
    {
        List<SetCard> CreateAllUniqueCombinations(GameSettings settings);
        bool VerifySet(List<SetCard> possibleSet);
        bool DoesFieldContainASet(List<SetCard> cards);
    }
}
