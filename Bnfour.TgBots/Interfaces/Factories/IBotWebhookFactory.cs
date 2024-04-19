using Bnfour.TgBots.Interfaces.Bots;

namespace Bnfour.TgBots.Interfaces.Factories;

/// <summary>
/// Provides webhook controls for bots configured to run.
/// </summary>
public interface IBotWebhookFactory
{
    /// <summary>
    /// Returns all enabled bots to manage their webhook state.
    /// </summary>
    public IEnumerable<IBotWebhook> GetBotWebhooks();
}
