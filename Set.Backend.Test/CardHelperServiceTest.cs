using Set.Backend.Models;
using Xunit;
using FluentAssertions;
using Set.Backend.Interfaces;
using System.Linq;
using Set.Backend.Services;

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
    }
}
