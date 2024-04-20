using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;

using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

public class UpdateHanderService(IBotFactory factory) : IUpdateHandlerService
{
    private readonly IBotFactory _factory = factory;

    public async Task HandleUpdate(string token, Update update)
    {
        var bot = _factory.GetBotByToken(token);
        if (bot != null)
        {
            await bot.HandleUpdate(update);
        }
        else
        {
            throw new NoSuchTokenException();
        }
    }
}
