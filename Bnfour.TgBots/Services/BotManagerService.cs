using Bnfour.TgBots.Interfaces;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

public class BotManagerService : IBotManagerService
{
    public Task HandleUpdate(string token, Update update)
    {
        throw new NotImplementedException();
    }

    public Task RemoveWebhooks()
    {
        throw new NotImplementedException();
    }

    public Task SetWebhooks()
    {
        throw new NotImplementedException();
    }
}
