using System.Net;
using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Controllers;

public class TelegramApiController : Controller
{
    private readonly IBotManagerService _service;

    public TelegramApiController(IBotManagerService service)
    {
        _service = service;
    }

    // POST /{bot token}

    /// <summary>
    /// Handles API calls from Telegram backend.
    /// </summary>
    /// <param name="token">Token of the bot the call is addressed to.
    /// Also used to verify it's actually Telegram calling us, as no one else is supposed to know it.</param>
    /// <param name="update">Update with a message from a user we want to process.</param>
    /// <returns>HTTP status code:<br/>200 if request is processed,<br/>404 if no bot with given token is found,<br/>500 if anything goes wrong.</returns>
    [HttpPost, Route("{token}")]
    public async Task<ActionResult> HandleTelegramCall(string token, [FromBody] Update update)
    {
        try
        {
            await _service.HandleUpdate(token, update);
            return Ok();
        }
        // TODO better handling when exceptions are thrown inside, at least:
        // 404 for wrong tokens, 400 bad request for missing data/inline queries for non-inline bot
        catch (NoSuchTokenException)
        {
            return NotFound();
        }
        catch
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
