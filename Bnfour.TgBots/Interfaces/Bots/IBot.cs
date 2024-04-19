using Telegram.Bot.Types;

namespace Bnfour.TgBots.Interfaces.Bots;

/// <summary>
/// Represents actual Telegram bot that handles incoming updates.
/// </summary>
public interface IBot
{
    public Task HandleUpdate(Update update);
}
