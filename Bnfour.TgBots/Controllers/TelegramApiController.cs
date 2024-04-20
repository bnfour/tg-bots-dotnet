using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Controllers;

/// <summary>
/// Controller to handle bot input as webhooks from Telegram's backend.
/// </summary>
public class TelegramApiController : Controller
{
    private readonly IUpdateHandlerService _service;

    public TelegramApiController(IUpdateHandlerService service)
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
    /// <returns>HTTP status code:<br/>- 200 if request is processed,<br/>- 404 if no bot with given token is found,<br/>
    /// - 400 if request is malformed,<br/>- 422 if request contains an inline query to a not-inline bot<br/>- 500 if anything else goes wrong.</returns>
    [HttpPost, Route("{token}")]
    public async Task<ActionResult> HandleTelegramCall(string token, [FromBody] Update update)
    {
        try
        {
            await _service.HandleUpdate(token, update);
            return Ok();
        }
        catch (NoSuchTokenException)
        {
            return NotFound();
        }
        catch (NotAnInlineBotException)
        {
            return UnprocessableEntity();
        }
        catch (NoRequiredDataException)
        {
            return BadRequest();
        }
        catch
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
