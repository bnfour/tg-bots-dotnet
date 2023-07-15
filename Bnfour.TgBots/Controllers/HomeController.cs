using System;
using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
