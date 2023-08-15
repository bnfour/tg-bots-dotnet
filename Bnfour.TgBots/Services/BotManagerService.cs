using Bnfour.TgBots.Bots;
using Bnfour.TgBots.Contexts;
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
    private IEnumerable<BotBase> ActiveBots => _bots.Where(b => b.Enabled);

    #region IBotManagerService implementation

    public BotManagerService(IOptions<ApplicationOptions> options,
        CatMacroBotContext catMacroBotContext)
    {
        _bots = new()
        {
            new LadderBot(options.Value.WebhookUrl, options.Value.LadderBotOptions),
            new CatMacroBot(options.Value.WebhookUrl, options.Value.CatMacroBotOptions, catMacroBotContext)
        };
    }

    public async Task HandleUpdate(string token, Update update)
    {
        var bot = ActiveBots.SingleOrDefault(b => b.IsToken(token));

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
        foreach (var bot in ActiveBots)
        {
            await bot.SetWebhook();
        }
    }

    public async Task RemoveWebhooks()
    {
        foreach (var bot in ActiveBots)
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
