using Set.Backend.Models;
using Xunit;
using FluentAssertions;
using Set.Backend.Interfaces;
using System.Linq;
using Set.Backend.Services;
using Shared.Tests;
using System.Collections.Generic;

namespace Set.Backend.Test
{
    public class CardHelperServiceTest
    {
        private readonly ICardHelperService _cardHelperService;
        public CardHelperServiceTest()
        {
            _cardHelperService = new CardHelperService();
        }

        [Fact]
        public void CreateAllUniqueCombinations_IncludeAllOptions()
        {
            var settings = new GameSettings { differentBorders = true, differentColors = true, differentCounts = true, differentShapes = true };

            var result = _cardHelperService.CreateAllUniqueCombinations(settings);

            // If all options are used, there should be exactly 81 unique combinations in total
            result.Count.Should().Be(81);
            result.Distinct().Count().Should().Be(81);
        }

        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, true, true, true)]
        public void CreateAllUniqueCombinations_ThreeOptions(bool useBorders, bool useColors, bool useCounts, bool useShapes)
        {
            var settings = new GameSettings { differentBorders = useBorders, differentColors = useColors, differentCounts = useCounts, differentShapes = useShapes };

            var result = _cardHelperService.CreateAllUniqueCombinations(settings);

            // If only 3 options are used, there should be 27 unique combinations, regardless of which parameter is set to false
            result.Count.Should().Be(27);
            result.Distinct().Count().Should().Be(27);
        }

        [Theory, AutoMoqData]
        public void VerifySet_SingleParameterVariation(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will go through each parameter option and will see if changes in them individually maintains the set

            // Different *colors*, rest remains the same
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[0], count[0]);
            var card3 = CreateCard(color[2], shape[0], border[0], count[0]);
            var setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();

            // Different *shapes*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[1], border[0], count[0]);
            card3 = CreateCard(color[0], shape[2], border[0], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();

            // Different *borders*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[0], border[1], count[0]);
            card3 = CreateCard(color[0], shape[0], border[2], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();

            // Different *counts*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[0], border[0], count[1]);
            card3 = CreateCard(color[0], shape[0], border[0], count[2]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void VerifySet_MultipleParameterVariation(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will go through each parameter option and will see if changes in them individually remains a set

            // Two parameters with variations
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[1], count[0]);
            var card3 = CreateCard(color[2], shape[0], border[2], count[0]);
            var setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();

            // Three parameters with variations and with different ordering
            card1 = CreateCard(color[1], shape[0], border[0], count[1]);
            card2 = CreateCard(color[2], shape[1], border[0], count[0]);
            card3 = CreateCard(color[0], shape[2], border[0], count[2]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();

            // Four parameters with variation and with different ordering
            card1 = CreateCard(color[0], shape[0], border[0], count[1]);
            card2 = CreateCard(color[2], shape[1], border[1], count[0]);
            card3 = CreateCard(color[1], shape[2], border[2], count[2]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void VerifySet_SingleFalse(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will go through each parameter option and will break them to test that they invalidate the set

            // Different *colors*, rest remains the same
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[0], count[0]);
            var card3 = CreateCard(color[1], shape[0], border[0], count[0]);
            var setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();

            // Different *shapes*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[0], border[0], count[0]);
            card3 = CreateCard(color[0], shape[1], border[0], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();

            // Different *borders*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[0], border[1], count[0]);
            card3 = CreateCard(color[0], shape[0], border[0], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();

            // Different *counts*, rest remains the same
            card1 = CreateCard(color[0], shape[0], border[0], count[2]);
            card2 = CreateCard(color[0], shape[0], border[0], count[1]);
            card3 = CreateCard(color[0], shape[0], border[0], count[2]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();
        }

        [Theory, AutoMoqData]
        public void VerifySet_MultipleFalse(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will go through each parameter option and will break them to test that they invalidate the set

            // 2 parameters false in different ways
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[0], count[0]);
            var card3 = CreateCard(color[1], shape[1], border[0], count[0]);
            var setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();

            // 3 parameters false in the same way
            card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            card2 = CreateCard(color[0], shape[0], border[0], count[0]);
            card3 = CreateCard(color[1], shape[1], border[1], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();

            // All parameters false
            card1 = CreateCard(color[0], shape[0], border[1], count[0]);
            card2 = CreateCard(color[1], shape[0], border[1], count[1]);
            card3 = CreateCard(color[0], shape[2], border[0], count[0]);
            setSumission = new List<SetCard> { card1, card2, card3 };

            _cardHelperService.VerifySet(setSumission).Should().BeFalse();
        }

        [Theory, AutoMoqData]
        public void DoesFieldContainASet_SingleSet(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will create a field with exactly one set, each separated from each other

            // The set
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[0], count[0]);
            var card3 = CreateCard(color[2], shape[0], border[0], count[0]);

            // Other cards which don't make a set
            var card4 = CreateCard(color[1], shape[1], border[1], count[1]);
            var card5 = CreateCard(color[1], shape[1], border[1], count[0]);
            var card6 = CreateCard(color[1], shape[1], border[0], count[0]);
            var card7 = CreateCard(color[1], shape[0], border[1], count[1]);
            var card8 = CreateCard(color[1], shape[0], border[0], count[1]);
            var card9 = CreateCard(color[2], shape[2], border[2], count[0]);
            var card10 = CreateCard(color[2], shape[2], border[0], count[0]);
            var card11 = CreateCard(color[2], shape[0], border[0], count[2]);
            var card12 = CreateCard(color[1], shape[2], border[1], count[2]);

            var field = new List<SetCard> { card1, card4, card5, card6, card2, card7, card8, card9, card3, card10, card11, card12 };

            _cardHelperService.DoesFieldContainASet(field).Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void DoesFieldContainASet_MultipleSets(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will create a field with multiple sets

            // The set
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[1], shape[0], border[0], count[0]);
            var card3 = CreateCard(color[2], shape[0], border[0], count[0]);

            // Another set, together with card1, meaning they overlap
            var card4 = CreateCard(color[1], shape[1], border[1], count[1]);
            var card5 = CreateCard(color[2], shape[2], border[2], count[2]);

            // Cards which don't make a set
            var card6 = CreateCard(color[1], shape[1], border[0], count[0]);
            var card7 = CreateCard(color[1], shape[0], border[1], count[1]);
            var card8 = CreateCard(color[1], shape[0], border[0], count[1]);
            var card9 = CreateCard(color[2], shape[2], border[2], count[0]);
            var card10 = CreateCard(color[2], shape[2], border[0], count[1]);
            var card11 = CreateCard(color[2], shape[0], border[0], count[0]);
            var card12 = CreateCard(color[1], shape[2], border[1], count[2]);

            var field = new List<SetCard> { card1, card2, card4, card6, card7, card8, card9, card3, card10, card11, card12, card5 };

            _cardHelperService.DoesFieldContainASet(field).Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void DoesFieldContainASet_NoSet(string[] color, string[] shape, string[] border, int[] count)
        {
            // This test will create a field with no sets
            var card1 = CreateCard(color[0], shape[0], border[0], count[0]);
            var card2 = CreateCard(color[0], shape[0], border[0], count[1]);
            var card3 = CreateCard(color[0], shape[0], border[1], count[0]);
            var card4 = CreateCard(color[0], shape[1], border[0], count[0]);
            var card5 = CreateCard(color[1], shape[0], border[0], count[0]);

            var card6 = CreateCard(color[0], shape[0], border[1], count[1]);
            var card7 = CreateCard(color[0], shape[1], border[1], count[0]);
            var card8 = CreateCard(color[1], shape[1], border[0], count[0]);

            var card9 = CreateCard(color[0], shape[1], border[1], count[1]);
            var card10 = CreateCard(color[1], shape[1], border[1], count[0]);

            var card11 = CreateCard(color[0], shape[1], border[0], count[1]);
            var card12 = CreateCard(color[1], shape[0], border[1], count[0]);

            var field = new List<SetCard> { card1, card2, card3, card4, card5, card6, card7, card8, card9, card10, card11, card12 };

            _cardHelperService.DoesFieldContainASet(field).Should().BeFalse();
        }

        private SetCard CreateCard(string color, string shape, string border, int count)
        {
            return new SetCard
            {
                Color = color,
                Shape = shape,
                Border = border,
                Count = count
            };
        }
    }
}
