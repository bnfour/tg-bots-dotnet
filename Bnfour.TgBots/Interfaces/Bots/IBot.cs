using Telegram.Bot.Types;

namespace Bnfour.TgBots.Interfaces.Bots;

/// <summary>
/// Represents actual Telegram bot that handles incoming updates.
/// </summary>
public interface IBot
{
    /// <summary>
    /// Handle an update, assuming it's legit (coming from Telegram backend),
    /// the caller should check that.
    /// </summary>
    /// <param name="update">Update to process.</param>
    public Task HandleUpdate(Update update);
}
