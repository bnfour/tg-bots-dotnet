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

    /// <summary>
    /// Constructor that defines the list of bots available.
    /// </summary>
    /// <param name="options">Application options to pass to bots.</param>
    /// <param name="catMacroBotContext">DB context for cat macro bot.</param>
    /// <param name="catMacroBotAdminHelperService">Helper service instance for the helper bot.</param>
    public BotManagerService(IOptions<ApplicationOptions> options,
        CatMacroBotContext catMacroBotContext,
        ICatMacroBotAdminHelperService catMacroBotAdminHelperService)
    {
        _bots =
        [
            new LadderBot(options.Value.LadderBotOptions),
            new CatMacroBot(options.Value.CatMacroBotOptions, catMacroBotContext, catMacroBotAdminHelperService)
        ];
    }

    #region IBotManagerService implementation

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

    public async Task<IEnumerable<BotInfoModel>> GetInfo()
    {
        var ret = new List<BotInfoModel>();
        foreach (var bot in _bots)
        {
            var model = await bot.GetModel();
            ret.Add(model);
        }
        return ret;
    }

    #endregion
}
