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

        private int numberOfCardsVisible = 12;
        private List<SetCardUiModel> uniqueCardCombinations;
        private string lineClass;
        private int numberOfSelected = 0;
        
        protected override void OnInitialized()
        {
            uniqueCardCombinations = _mapper.Map<List<SetCard>, List<SetCardUiModel>>(_cardHelperService.CreateAllUniqueCombinations());
            lineClass = _uiHelperService.GetLineClass(numberOfCardsVisible);

        }

        private void ProcessSelection(SetCardUiModel setCard)
        {
            var toBeSelected = setCard.BackGroundColor == "white" ? true : false;

            setCard.BackGroundColor = toBeSelected ? "yellow" : "white";
            numberOfSelected += toBeSelected ? 1 : -1;

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
