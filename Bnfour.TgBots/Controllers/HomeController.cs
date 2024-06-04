using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Models;

using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

/// <summary>
/// Controller to handle the home/landing page
/// that displays bot status along with some static content.
/// </summary>
public class HomeController(IBotInfoProviderService infoProvider) : Controller
{
    private readonly IBotInfoProviderService _infoProvider = infoProvider;

    // GET /

    public async Task<IActionResult> Index()
    {
        var model = new HomeModel
        {
            Bots = await _infoProvider.GetInfo()
        };
        return View(model);
    }

    // GET /favicon.ico
    // returns an almost empty 404 response as opposed to a full blown page
    // for other errors

    [HttpGet, Route("/favicon.ico"), SkipStatusCodePages]
    public IActionResult Plain404ForFavicon()
    {
        return NotFound("Nothing here");
    }
}
