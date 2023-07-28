using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

public class HomeController : Controller
{
    private readonly IBotInfoProviderService _infoProvider;

    public HomeController(IBotInfoProviderService infoProvider)
    {
        _infoProvider = infoProvider;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeModel
        {
            Bots = await _infoProvider.GetInfo()
        };
        return View(model);
    }
}
