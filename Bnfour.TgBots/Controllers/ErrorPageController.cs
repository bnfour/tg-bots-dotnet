using Bnfour.TgBots.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

/// <summary>
/// Controller that displays custom error page on GET request errors.
/// (Mostly 404, also 405 if actual API path is requested).
/// </summary>
public class ErrorPageController : Controller
{
    // GET /status/{code}
    // should be called internally via re-execution to display an error page
    // GET only, other verbs are considered API usage and return empty responses
    // with nothing but codes

    [HttpGet, Route("/status/{statusCode}")]
    public IActionResult DisplayError(int statusCode)
    {
        return View(new ErrorModel(statusCode));
    }
}
