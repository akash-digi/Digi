namespace Digi.WebUI.Services.WebClientServices;

public class WebClientService : IWebClientService
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<string> GetHtmlString(string requestUrl)
    {
        if (string.IsNullOrEmpty(requestUrl))
        {
            throw new ArgumentNullException("requestUrl");
        }
        string responseHtml = await _httpClient.GetStringAsync(requestUrl);

        return responseHtml;
    }
}
