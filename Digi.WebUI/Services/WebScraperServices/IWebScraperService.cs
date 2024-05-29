using Digi.WebUI;

namespace Digi.WebUI.Services.WebScraperServices;

public interface IWebScraperService
{
    Task<KeywordResponse> SearchByKeword(KeywordRequest keywordRequest);
}
