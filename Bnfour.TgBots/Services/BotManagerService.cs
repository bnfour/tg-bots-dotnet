using Bnfour.TgBots.Bots;
using Bnfour.TgBots.Exceptions;
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
            throw new NoSuchTokenException();
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

    // TODO make this method async somehow?
    // it's not async and returns Task.FromResult just to prevent CS1998
    // using async in ForEach apparently does not make the whole method async
    public Task<IEnumerable<BotInfoModel>> GetInfo()
    {
        var ret = new List<BotInfoModel>();
        _bots.ForEach(async b =>
        {
            ret.Add(await b.GetModel());
        });
        return Task.FromResult(ret.AsEnumerable());
    }

    #endregion
}
