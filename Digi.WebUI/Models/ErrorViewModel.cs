namespace Digi.WebUI;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public List<string> ErrorMessages { get; set; } = [];

}
