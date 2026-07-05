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
public class BotFactory(
    IOptions<ApplicationOptions> options,
    CatMacroBotContext catMacroContext,
    ICatMacroBotAdminHelperService catMacroHelper) : IBotFactory, IBotInfoFactory, IBotWebhookFactory
{
    public IBot? GetBotByToken(string token)
        => EnabledBots().FirstOrDefault(b => b.IsToken(token));

    public IEnumerable<IBotInfo> GetBotInfo()
        => AllBots().Cast<IBotInfo>();
    

    public IEnumerable<IBotWebhook> GetBotWebhooks()
        => EnabledBots().Cast<IBotWebhook>();


    // _the_ place where the bots are defined
    private IEnumerable<BotBase> AllBots()
    {
        yield return new LadderBot(options.Value.LadderBotOptions);
        yield return new CatMacroBot(options.Value.CatMacroBotOptions, catMacroContext, catMacroHelper);
    }

    private IEnumerable<BotBase> EnabledBots()
        => AllBots().Where(b => b.Enabled);
}
