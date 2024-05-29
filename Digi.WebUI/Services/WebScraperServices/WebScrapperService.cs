
using Digi.WebUI;
using Digi.WebUI.Services.WebClientServices;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Xml;
using System.Xml.Linq;

namespace Digi.WebUI.Services.WebScraperServices;

public class WebScrapperService(IWebClientService webClientService, IOptions<AppConfig> appConfigOptions, ILogger<WebScrapperService> logger) : IWebScraperService
{
    private readonly IWebClientService _webClientService = webClientService;
    private readonly AppConfig _appConfig = appConfigOptions.Value;
    private readonly ILogger<WebScrapperService> _logger = logger;

    public async Task<KeywordResponse> SearchByKeword(KeywordRequest keywordRequest)
    {
        List<KeywordResult> listOfFinalKeywordResults = [];
        try
        {

            foreach (string url in _appConfig.WebScrapUrls)
            {   // get html code from web client utility and process html code to get all anchor tag listOfKeywordResults containing keyword
                IEnumerable<KeywordResult> listOfExtractedLinks = await GetKeywordResults(url, keywordRequest.Keyword);

                // merge all list in one list 
                listOfFinalKeywordResults.AddRange(listOfExtractedLinks);
            }
        }
        catch (Exception ex)
        {
            return new KeywordResponse { IsSuccessed = false, Errors = [ex.Message], Keyword = keywordRequest.Keyword };
        }

        return new KeywordResponse { IsSuccessed = true, ListOfKeywordResults = listOfFinalKeywordResults, Keyword = keywordRequest.Keyword };
    }

    private async Task<IEnumerable<KeywordResult>> GetKeywordResults(string url, string keyword)
    {
        var listOfKeywordResults = new List<KeywordResult>();

        try
        {
            var response = await _webClientService.GetHtmlString(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

            if (linkNodes is null || linkNodes.Count == 0)
            {
                return listOfKeywordResults;
            }

            foreach (var linkNode in linkNodes)
            {
                var hrefValue = linkNode.GetAttributeValue("href", string.Empty);
                if (hrefValue is null) continue;

                if (!hrefValue.ToLower().Contains(keyword.ToLower())) continue;

                KeywordResult? result = GetKeyResultFromNode(linkNode, linkNode);
                if (result == null) continue;

                result.BaseUrl = url;
                listOfKeywordResults.Add(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Cannot process {url} for keyword {keyword}. Error: {ex.Message}");
        }

        return listOfKeywordResults;
    }

    public static KeywordResult? GetKeyResultFromNode(HtmlNode node, HtmlNode linkNode)
    {
        KeywordResult? keywordResult = null;
        if (node is null) return keywordResult;

        var imgNodes = node.Descendants("img");

        // seeach inside archor tag if img is found
        var imgNode = imgNodes.FirstOrDefault(x => x.GetAttributeValue("data-src", string.Empty) == string.Empty);
        if (imgNode != null)
        {
            var imageSrc = imgNode.GetAttributeValue("src", string.Empty);

            List<HtmlNode> htmlNodes = new List<HtmlNode>();
            GetDescendentNodesWithText(node, htmlNodes);
            if (htmlNodes.Count > 0)
            {
                keywordResult = new KeywordResult
                {
                    Title = htmlNodes[0].InnerText,
                    ImageUrl = imageSrc,
                    LinkUrl = linkNode.GetAttributeValue("href", string.Empty),
                    Description = htmlNodes[1].InnerText,
                };
            }
        }
        else
        {
            HtmlNode? parentNode = GetFirstAncestorNodeWithImage(node);
            if (parentNode != null)
                keywordResult = GetKeyResultFromNode(parentNode, linkNode);
        }

        return keywordResult;
    }

    private static HtmlNode? GetFirstAncestorNodeWithImage(HtmlNode node)
    {

        // Find the parent node which contains img tag and extract details
        var parent = node.Ancestors().FirstOrDefault();

        if (parent is null) return null;

        // seeach inside archor tag if img is found
        var imgNode = parent.Descendants("img").FirstOrDefault();

        if (imgNode is not null)
        {
            return parent;
        }

        return GetFirstAncestorNodeWithImage(parent);
    }



    private static void GetDescendentNodesWithText(HtmlNode node, List<HtmlNode> nodesWithText)
    {
        if (!string.IsNullOrWhiteSpace(node.InnerText) && node.NodeType == HtmlNodeType.Element)
        {
            nodesWithText.Add(node);
        }

        foreach (var childNode in node.ChildNodes)
        {
            GetDescendentNodesWithText(childNode, nodesWithText);
        }
    }

    private async Task<KeywordResult> GetPageDetails(string url)
    {
        var details = new KeywordResult();

        try
        {
            var response = await _webClientService.GetHtmlString(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
            var imageNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
            var priceNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='product:price:amount']");

            //details.Title = titleNode?.InnerText.Trim();
            //details.Description = descriptionNode?.GetAttributeValue("content", string.Empty).Trim();
            //details.Image = imageNode?.GetAttributeValue("content", string.Empty).Trim();
            //details.Price = priceNode?.GetAttributeValue("content", string.Empty).Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching details from {url}: {ex.Message}");
        }

        return details;
    }
}

