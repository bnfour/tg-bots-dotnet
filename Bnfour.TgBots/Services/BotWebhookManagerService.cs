using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;

namespace Bnfour.TgBots.Services;

/// <summary>
/// Manages the webhooks for the bots on the application startup and shutdown.
/// </summary>
/// <param name="factory">Factory to get bot instances to manage from.</param>
public class BotWebhookManagerService(IBotWebhookFactory factory) : IBotWebhookManagerService
{
    private readonly IBotWebhookFactory _factory = factory;

    public async Task RemoveWebhooks()
    {
        foreach (var bot in _factory.GetBotWebhooks())
        {
            await bot.RemoveWebhook();
        }
    }

    public async Task SetWebhooks()
    {
        foreach (var bot in _factory.GetBotWebhooks())
        {
            await bot.SetWebhook();
        }
    }
}
