using Set.Backend.Interfaces;
using Set.Backend.Models;
using System;
using System.Collections.Generic;

namespace Set.Backend.Services
{
    public class CardHelperService : ICardHelperService
    {
        private static readonly Random rng = new Random();

        public List<SetCard> CreateAllUniqueCombinations(GameSettings settings)
        {
            var cardOptions = GetCardOptions(settings);
            var uniqueSetCardCombinations = new List<SetCard>();

            foreach (var shape in cardOptions.Shapes)
            {
                foreach (var Border in cardOptions.Borders)
                {
                    foreach (var color in cardOptions.Colors)
                    {
                        foreach (var count in cardOptions.Counts)
                        {
                            uniqueSetCardCombinations.Add(
                                new SetCard
                                {
                                    Shape = shape,
                                    Border = Border,
                                    Color = color,
                                    Count = count,
                                }
                            );
                        }
                    }
                }
            }

            return Shuffle(uniqueSetCardCombinations);
        }

        private SetCardOptions GetCardOptions(GameSettings settings)
        {
            return new SetCardOptions
            {
                Shapes = settings.differentShapes ? new string[] { "circle", "square", "star" } : new string[] { "square" },
                Borders = settings.differentBorders ? new string[] { "dotted", "solid", "noBorder" } : new string[] { "noBorder" },
                Colors = settings.differentColors ? new string[] { "red", "green", "blue" } : new string[] { "blue" },
                Counts = settings.differentCounts ? new int[] { 1, 2, 3 } : new int[] { 1 }
            };
        }

        private List<SetCard> Shuffle(List<SetCard> setCards)
        {
            int n = setCards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = setCards[k];
                setCards[k] = setCards[n];
                setCards[n] = value;
            }

            return setCards;
        }

        public bool VerifySet(List<SetCard> possibleSet)
        {
            var isColorCorrect = (possibleSet[0].Color == possibleSet[1].Color && possibleSet[0].Color == possibleSet[2].Color) ||
                (possibleSet[0].Color != possibleSet[1].Color && possibleSet[0].Color != possibleSet[2].Color && possibleSet[1].Color != possibleSet[2].Color);

            var isShapeCorect = (possibleSet[0].Shape == possibleSet[1].Shape && possibleSet[0].Shape == possibleSet[2].Shape) ||
                (possibleSet[0].Shape != possibleSet[1].Shape && possibleSet[0].Shape != possibleSet[2].Shape && possibleSet[1].Shape != possibleSet[2].Shape);

            var isBorderCorrect = (possibleSet[0].Border == possibleSet[1].Border && possibleSet[0].Border == possibleSet[2].Border) ||
                (possibleSet[0].Border != possibleSet[1].Border && possibleSet[0].Border != possibleSet[2].Border && possibleSet[1].Border != possibleSet[2].Border);

            var isCountCorrect = (possibleSet[0].Count == possibleSet[1].Count && possibleSet[0].Count == possibleSet[2].Count) ||
                (possibleSet[0].Count != possibleSet[1].Count && possibleSet[0].Count != possibleSet[2].Count && possibleSet[1].Count != possibleSet[2].Count);

            return isColorCorrect && isShapeCorect && isBorderCorrect && isCountCorrect;
        }

        public bool DoesFieldContainASet(List<SetCard> cards)
        {
            // Tripple loop to go through each unique possible combination of cards to check for a set
            // The order of the cards doesn't matter, so always starting j on the right of i and k on the right of j
            // as all earlier possible combinations have already been tried
            for (int i = 0; i < cards.Count - 2; i++)
            {
                for (int j = i + 1; j < cards.Count - 1; j++)
                {
                    for (int k = j + 1; k < cards.Count; k++)
                    {
                        if (VerifySet(new List<SetCard> { cards[i], cards[j], cards[k] }))
                        {
                            return true;
                        }
                    }
                }
            }

            // If no set has been found in all the possibilities, then return false
            return false;
        }
    }
}
