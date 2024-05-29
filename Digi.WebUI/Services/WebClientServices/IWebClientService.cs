namespace Digi.WebUI.Services.WebClientServices;

public interface IWebClientService
{
    Task<string> GetHtmlString(string requestUrl);
}
