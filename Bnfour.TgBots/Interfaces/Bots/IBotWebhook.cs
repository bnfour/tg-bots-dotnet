namespace Bnfour.TgBots.Interfaces.Bots;

/// <summary>
/// Used to manage the bot's webhooks.
/// </summary>
public interface IBotWebhook
{
    /// <summary>
    /// Point Telegram API to this app's endpoint.
    /// </summary>
    public Task SetWebhook();

    /// <summary>
    /// Release the webhook acquired earlier.
    /// </summary>
    public Task RemoveWebhook();
}
