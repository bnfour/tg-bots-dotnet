using Telegram.Bot.Types;

namespace Bnfour.TgBots.Interfaces;

/// <summary>
/// Used to manage multiple (small) bots within one application.
/// </summary>
public interface IBotManagerService
{
    /// <summary>
    /// Set the webhooks for all configured (and thus enabled) bots.
    /// </summary>
    Task SetWebhooks();

    /// <summary>
    /// Remove all set webhooks.
    /// </summary>
    Task RemoveWebhooks();

    /// <summary>
    /// Handle an update, presumably from Telegram backend.
    /// </summary>
    /// <param name="token">Token from URL, used for auth and selecting a matching bot.</param>
    /// <param name="update">Update to handle by one of the bots.</param>
    Task HandleUpdate(string token, Update update);
}
