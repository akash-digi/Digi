namespace Digi.WebUI;

public class KeywordResponse
{
    public string Keyword { get; set; }
    public bool IsSuccessed { get; set; }
    public List<KeywordResult> ListOfKeywordResults { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}


public class KeywordResult
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string BaseUrl { get; set; }
    public string LinkUrl { get; set; }
    public string ImageUrl { get; set; }
}
