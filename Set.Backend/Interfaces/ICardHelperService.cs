using Set.Backend.Models;
using System.Collections.Generic;

namespace Set.Backend.Interfaces
{
    public interface ICardHelperService
    {
        List<SetCard> CreateAllUniqueCombinations(GameSettings settings);
        bool DoesFieldContainASet(List<SetCard> cards);
        bool VerifySet(List<SetCard> possibleSet);
    }
}