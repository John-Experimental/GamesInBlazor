﻿using SETInBlazor.Services.Interfaces;

namespace SETInBlazor.Services
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
    }
}