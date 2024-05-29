using Digi.WebUI.Constants;
using Digi.WebUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Digi.WebUI.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public GlobalExceptionFilter(IModelMetadataProvider modelMetadataProvider)
    {
        _modelMetadataProvider = modelMetadataProvider;
    }
    public void OnException(ExceptionContext context)
    {
        ViewResult viewResult = new ViewResult() { ViewName = "Error" };
        viewResult.ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(_modelMetadataProvider, context.ModelState);
        viewResult.ViewData.Add("Exception", new ErrorViewModel() { ErrorMessages = new List<string>() { ErrorConstant.INTERNAL_SERVER_ERROR_MESSAGE } });

        // Here we can pass additional detailed data via ViewData
        context.ExceptionHandled = true; // mark exception as handled

        context.Result = viewResult;
    }
}
