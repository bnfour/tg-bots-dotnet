using Bnfour.TgBots.Interfaces.Services;

using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

public class UpdateHanderService : IUpdateHandlerService
{
    public async Task HandleUpdate(string token, Update update)
    {
        // TODO instantiate the correct bot by token and handle the update
        throw new NotImplementedException();
    }
}
