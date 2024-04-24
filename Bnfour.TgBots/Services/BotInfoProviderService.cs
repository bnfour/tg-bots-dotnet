using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Models;

namespace Bnfour.TgBots.Services;

/// <summary>
/// Provides the bot info for the status page.
/// </summary>
/// <param name="factory">Factory to provide bot instances to get info from.</param>
public class BotInfoProviderService(IBotInfoFactory factory) : IBotInfoProviderService
{
    private readonly IBotInfoFactory _factory = factory;

    public async Task<IEnumerable<BotInfoModel>> GetInfo()
    {
        var ret = new List<BotInfoModel>();
        foreach (var bot in _factory.GetBotInfo())
        {
            var info = await bot.GetModel();
            ret.Add(info);
        }
        return ret;
    }
}
