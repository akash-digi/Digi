using Digi.WebUI.Services.WebClientServices;
using Digi.WebUI.Services.WebScraperServices;

namespace Digi.WebUI.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddScoped<IWebClientService, WebClientService>();
        services.AddScoped<IWebScraperService, WebScrapperService>();

        return services;
    }
}
