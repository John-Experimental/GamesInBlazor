using AutoMapper;
using Microsoft.AspNetCore.Components;
using SETInBlazor.Models;
using SETInBlazor.Services.Interfaces;
using SETLogicLibrary.Interfaces;
using SETLogicLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace SETInBlazor.Pages
{
    public partial class SET
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

        protected override void OnInitialized()
        {
            difficultyVariation = difficultyVariation ?? "NORMAL";
            var settings = new GameSettings(difficultyVariation);
            numberOfCardsVisible = settings.numberOfCardsVisible;

            uniqueCardCombinations = _mapper.Map<List<SetCard>, List<SetCardUiModel>>(_cardHelperService.CreateAllUniqueCombinations(settings));

            lineClass = _uiHelperService.GetLineClass(numberOfCardsVisible);
        }

        private void ProcessSelection(SetCardUiModel setCard)
        {
            var toBeSelected = setCard.BackGroundColor == "white" ? true : false;
            numberOfSelected += toBeSelected ? 1 : -1;

            setCard.BackGroundColor = toBeSelected ? "yellow" : "white";

            if (numberOfSelected == 3)
            {
                var potentialSet = _mapper.Map<List<SetCardUiModel>, List<SetCard>>(uniqueCardCombinations.Where(card => card.BackGroundColor == "yellow").ToList());
                var isSet = _cardHelperService.VerifySet(potentialSet);

                if (isSet)
                {
                    uniqueCardCombinations.RemoveAll(card => card.BackGroundColor == "yellow");
                    numberOfSelected = 0;
                }
            };

        }
    }
}
