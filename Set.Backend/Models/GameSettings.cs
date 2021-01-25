namespace Set.Backend.Models
{
    public class GameSettings
    {
        public bool differentShapes { get; set; } = true;
        public bool differentBorders { get; set; } = true;
        public bool differentColors { get; set; } = true;
        public bool differentCounts { get; set; } = true;
        public int numberOfCardsVisible = 12;

        /// <summary>
        /// Initializes the required variables for a game based on a difficulty level parameter
        /// </summary>
        /// <param name="difficultyVariation">Standard if no parameter is passed: "NORMAL" ==> Acceptable options: "EASY"</param>
        public GameSettings(string difficultyVariation)
        {
            // Defined this as a switch because in the future I may want to expand the number of difficulty variations
            // The variation of these parameters can allow for multiple 'practise' environments for kids, or more difficult levels for adults
            switch (difficultyVariation.ToUpper())
            {
                case "EASY":
                    differentBorders = false;
                    numberOfCardsVisible = 9;
                    break;
                default:                    
                    break;
            }
        }

        public GameSettings()
        {
            // Currently sets the standard values of the parameters according to the 'normal' SET game rules
        }

    }
}
