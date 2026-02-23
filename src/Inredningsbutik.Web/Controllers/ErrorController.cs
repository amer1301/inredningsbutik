using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inredningsbutik.Web.Models;

namespace Inredningsbutik.Web.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // Hanterar 500 (Unhandled exceptions)
    [Route("")]
    public IActionResult Index()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error != null)
        {
            _logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception on path {Path}",
                exceptionFeature.Path);
        }

        var model = new ErrorViewModel
        {
            StatusCode = 500,
            OriginalPath = exceptionFeature?.Path,
            TraceId = HttpContext.TraceIdentifier
        };

        Response.StatusCode = 500;

        return View(model);
    }

    // Hanterar 404, 403, 401 etc.
    [Route("{statusCode:int}")]
    public IActionResult Status(int statusCode)
    {
        var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var originalPath = statusFeature?.OriginalPath ?? HttpContext.Request.Path.Value;
        var originalQuery = statusFeature?.OriginalQueryString ?? "";

        if (statusCode == 404)
            _logger.LogWarning("HTTP 404 Not Found on {Path}{Query}", originalPath, originalQuery);
        else if (statusCode == 403)
            _logger.LogWarning("HTTP 403 Forbidden on {Path}{Query}", originalPath, originalQuery);
        else if (statusCode == 401)
            _logger.LogWarning("HTTP 401 Unauthorized on {Path}{Query}", originalPath, originalQuery);
        else
            _logger.LogWarning("HTTP {StatusCode} on {Path}{Query}", statusCode, originalPath, originalQuery);

        var model = new ErrorViewModel
        {
            StatusCode = statusCode,
            OriginalPath = originalPath,
            TraceId = HttpContext.TraceIdentifier
        };

        return View("Index", model);
    }
}