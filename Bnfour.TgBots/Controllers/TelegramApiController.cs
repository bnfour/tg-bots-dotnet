using Microsoft.AspNetCore.Mvc;

namespace Bnfour.TgBots.Controllers;

public class TelegramApiController : Controller
{
    // POST /{bot token}
    // TODO maybe consider routing different from og version

    [HttpPost, Route("{token}")]
    public async Task<ActionResult> HandleTelegramCall(string token) //, TODO [FromBody] telegram update 
    {
        return StatusCode(418, token);
    }
}
