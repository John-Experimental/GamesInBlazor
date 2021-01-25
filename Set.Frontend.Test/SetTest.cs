using AutoMapper;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Set.Backend.Interfaces;
using Set.Backend.Services;
using Set.Frontend.Interfaces;
using Set.Frontend.Pages;
using Set.Frontend.Services;
using Xunit;
using FluentAssertions;

namespace Set.Frontend.Test
{
    public class SetTest
    {
        private readonly TestContext _ctx;
        public SetTest()
        {
            _ctx = new TestContext();

            // Register services
            _ctx.Services.AddSingleton<ICardHelperService>(new CardHelperService());
            _ctx.Services.AddSingleton<IUiHelperService>(new UiHelperService());
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            });
            _ctx.Services.AddSingleton(configuration.CreateMapper());
        }

        [Fact]
        public void CardHighlightWorks()
        {
            var page = _ctx.RenderComponent<SetPage>();
            var card = page.Find(".card");

            // Originally the cards all have a white background
            card.GetAttribute("style").Should().Contain("background-color:white");

            // Once selected they change color to yellow to indicate that they've been selected
            card.Click();
            card.GetAttribute("style").Should().Contain("background-color:yellow");

            // Once clicked again they're de-selected and should return to a white background color
            card.Click();
            card.GetAttribute("style").Should().Contain("background-color:white");
        }
    }
}
