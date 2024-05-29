using Digi.WebUI.Constants;
using System.ComponentModel.DataAnnotations;

namespace Digi.WebUI;

public class KeywordRequest
{
    [Required]
    [MinLength(ApplicationConstant.KEYWORD_MIN_LENGTH)]
    public string Keyword { get; set; }
}
