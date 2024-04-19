using Bnfour.TgBots.Interfaces.Bots;

namespace Bnfour.TgBots.Interfaces.Factories;

/// <summary>
/// Provides a single bot with the given token, or null if no such bot found.
/// </summary>
public interface IBotFactory
{
    /// <summary>
    /// Get the bot by its token, to handle the incoming update.
    /// </summary>
    /// <param name="token">Token to match with the bot.</param>
    /// <returns>The bot, or null if no bot with provided token is found.</returns>
    public IBot? GetBotByToken(string token);
}
