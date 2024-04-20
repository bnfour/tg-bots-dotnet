using Microsoft.Extensions.Options;
using Bnfour.TgBots.Bots;
using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Interfaces.Bots;
using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Options;

namespace Bnfour.TgBots.Factories;

/// <summary>
/// Provides the bots, only by an interface that is used in the caller.
/// </summary>
public class BotFactory : IBotFactory, IBotInfoFactory, IBotWebhookFactory
{
    private readonly ApplicationOptions _options;

    private readonly CatMacroBotContext _catMacroBotContext;
    private readonly ICatMacroBotAdminHelperService _catMacroHelper;

    public BotFactory(
        IOptions<ApplicationOptions> options,
        CatMacroBotContext catMacroContext,
        ICatMacroBotAdminHelperService catMacroHelper)
    {
        _options = options.Value;

        _catMacroBotContext = catMacroContext;
        _catMacroHelper = catMacroHelper;
    }

    public IBot? GetBotByToken(string token)
    {
        foreach (var bot in EnabledBots())
        {
            if (bot.IsToken(token))
            {
                return bot;
            }
        }

        return null;
    }

    public IEnumerable<IBotInfo> GetBotInfo()
    {
        return AllBots().Cast<IBotInfo>();
    }

    public IEnumerable<IBotWebhook> GetBotWebhooks()
    {
        return EnabledBots().Cast<IBotWebhook>();
    }

    // _the_ place where the bots are defined
    private IEnumerable<BotBase> AllBots()
    {
        yield return new LadderBot(_options.LadderBotOptions);
        yield return new CatMacroBot(_options.CatMacroBotOptions, _catMacroBotContext, _catMacroHelper);
    }

    private IEnumerable<BotBase> EnabledBots()
        => AllBots().Where(b => b.Enabled);
}
