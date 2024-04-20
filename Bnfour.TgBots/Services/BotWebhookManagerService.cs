using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;

namespace Bnfour.TgBots.Services;

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
