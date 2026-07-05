using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Interfaces.Factories;
using Bnfour.TgBots.Interfaces.Services;

using Telegram.Bot.Types;

namespace Bnfour.TgBots.Services;

/// <summary>
/// Service to handle incoming updates.
/// </summary>
/// <param name="factory">Factory used to get the needed bot instance from.</param>
public class UpdateHandlerService(IBotFactory factory) : IUpdateHandlerService
{
    public async Task HandleUpdate(string token, Update update)
    {
        var bot = factory.GetBotByToken(token);
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
