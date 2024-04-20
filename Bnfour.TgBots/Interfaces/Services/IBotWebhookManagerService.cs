namespace Bnfour.TgBots.Interfaces.Services;

public interface IBotWebhookManagerService
{
    /// <summary>
    /// Set the webhooks for all configured (and thus enabled) bots.
    /// </summary>
    Task SetWebhooks();

    /// <summary>
    /// Remove all set webhooks.
    /// </summary>
    Task RemoveWebhooks();
}
