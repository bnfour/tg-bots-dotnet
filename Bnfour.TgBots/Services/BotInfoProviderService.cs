using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Models;

namespace Bnfour.TgBots.Services;

public class BotInfoProviderService : IBotInfoProviderService
{
    public async Task<IEnumerable<BotInfoModel>> GetInfo()
    {
        // TODO instantiate _all_ bots (even inactive) to get their basic info
        throw new NotImplementedException();
    }
}
