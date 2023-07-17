using Bnfour.TgBots.Bots;
using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

public class BotManagerService : IBotManagerService
{
    /// <summary>
    /// List of bots available in this app.
    /// </summary>
    private readonly List<BotBase> _bots;

    /// <summary>
    /// Shorthand property to query only enabled bots.
    /// </summary>
    private IEnumerable<BotBase> _activeBots => _bots.Where(b => b.Enabled);

    public BotManagerService(IOptions<ApplicationOptions> options)
    {
        // TODO fill with bot instances
        _bots = new()
        {
            // create and configure bots here
        };
    }

    public async Task HandleUpdate(string token, Update update)
    {
        var bot = _activeBots.SingleOrDefault(b => b.IsToken(token));

        if (bot != null)
        {
            await bot.HandleUpdate(update);
        }
        else
        {
            // TODO throw something to return 404 status code
        }
    }

    public async Task SetWebhooks()
    {
        foreach (var bot in _activeBots)
        {
            await bot.SetWebhook();
        }
    }

    public async Task RemoveWebhooks()
    {
        foreach (var bot in _activeBots)
        {
            await bot.RemoveWebhook();
        }
    }
}
