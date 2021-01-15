using SETLogicLibrary.Models;
using System;
using System.Collections.Generic;

namespace SETLogicLibrary.Interfaces
{
    public interface ICardHelperService
    {
        List<SetCard> CreateAllUniqueCombinations(GameSettings settings);
        bool VerifySet(List<SetCard> possibleSet);
    }
}
