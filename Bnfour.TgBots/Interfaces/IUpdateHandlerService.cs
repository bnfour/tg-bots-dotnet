using Telegram.Bot.Types;

namespace Bnfour.TgBots.Interfaces;

public interface IUpdateHandlerService
{
    /// <summary>
    /// Handle an update, presumably from Telegram backend.
    /// </summary>
    /// <param name="token">Token from URL, used for auth and selecting a matching bot.</param>
    /// <param name="update">Update to handle by one of the bots.</param>
    Task HandleUpdate(string token, Update update);
}
