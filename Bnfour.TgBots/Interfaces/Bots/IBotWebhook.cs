namespace Bnfour.TgBots.Interfaces.Bots;

/// <summary>
/// Used to manage the bot's webhooks.
/// </summary>
public interface IBotWebhook
{
    public Task SetWebhook();

    public Task RemoveWebhook();
}
