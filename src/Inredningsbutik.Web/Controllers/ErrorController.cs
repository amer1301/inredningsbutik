using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inredningsbutik.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error")]
    public IActionResult Index()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (feature?.Error is not null)
        {
            _logger.LogError(feature.Error,
                "Unhandled exception on path {Path}",
                feature.Path);
        }

        // Ex: visa correlation-id i view (bra i rapport/video)
        ViewBag.TraceId = HttpContext.TraceIdentifier;

        return View();
    }

    [Route("Error/{statusCode:int}")]
    public IActionResult Status(int statusCode)
    {
        var original = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var originalPath = original?.OriginalPath ?? HttpContext.Request.Path.Value;
        var originalQuery = original?.OriginalQueryString ?? "";

        if (statusCode == 404)
            _logger.LogWarning("HTTP 404 Not Found on {Path}{Query}", originalPath, originalQuery);
        else if (statusCode == 403)
            _logger.LogWarning("HTTP 403 Forbidden on {Path}{Query}", originalPath, originalQuery);
        else
            _logger.LogWarning("HTTP {StatusCode} on {Path}{Query}", statusCode, originalPath, originalQuery);

        ViewBag.StatusCode = statusCode;
        ViewBag.TraceId = HttpContext.TraceIdentifier;
        ViewBag.OriginalPath = originalPath;

        return View();
    }
}
