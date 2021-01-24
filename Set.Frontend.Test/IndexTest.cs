using Bunit;
using Set.Frontend.Pages;
using Xunit;

namespace Set.Frontend.Test
{
    public class SETTest : TestContext
    {
        [Fact]
        public void CardHighlightWorks()
        {
            var page = RenderComponent<SetPage>();
        }
    }
}
