using Bnfour.TgBots.Bots;
using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Models;
using Bnfour.TgBots.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

/// <summary>
/// Class that holds all the bots and manages their communication with the outside world.
/// </summary>
public class BotManagerService : IBotManagerService, IBotInfoProviderService
{
    /// <summary>
    /// List of bots available in this app.
    /// </summary>
    private readonly List<BotBase> _bots;

    /// <summary>
    /// Shorthand property to query only enabled bots.
    /// </summary>
    private IEnumerable<BotBase> _activeBots => _bots.Where(b => b.Enabled);

    #region IBotManagerService implementation

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

    #endregion

    #region IBotInfoProviderService implementation

    public async Task<IEnumerable<BotInfoModel>> GetInfo()
    {
        // TODO actual bot info
        return new List<BotInfoModel>
            {
                new BotInfoModel { IsOnline = true, Username = "bndebug_bot" },
                new BotInfoModel { IsOnline = false, Username = null }
            }
            .AsEnumerable();
    }

    #endregion
}
