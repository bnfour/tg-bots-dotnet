using Bnfour.TgBots.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // TODO grab this from BotManager
        var model = new HomeModel
        {
            Bots = new List<BotInfoModel>
            {
                new BotInfoModel { IsOnline = true, Username = "bndebug_bot" },
                new BotInfoModel { IsOnline = false, Username = null }
            }
        };
        return View(model);
    }
}
