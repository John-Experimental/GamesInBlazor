using AutoMapper;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SETInBlazor.Services;
using SETInBlazor.Services.Interfaces;
using SETLogicLibrary.Interfaces;
using SETLogicLibrary.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SETInBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ICardHelperService, CardHelperService>();
            builder.Services.AddScoped<IUiHelperService, UiHelperService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            });
            builder.Services.AddSingleton(configuration.CreateMapper());

            await builder.Build().RunAsync();
        }
    }
}
