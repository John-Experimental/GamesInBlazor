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
