using Bnfour.TgBots.Interfaces.Bots;

namespace Bnfour.TgBots.Interfaces.Factories;

/// <summary>
/// Provides webhook controls for bots configured to run.
/// </summary>
public interface IBotWebhookFactory
{
    public IEnumerable<IBotWebhook> GetBotWebhooks();
}
