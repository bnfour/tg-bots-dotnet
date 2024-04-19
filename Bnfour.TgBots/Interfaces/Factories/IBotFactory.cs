using Bnfour.TgBots.Interfaces.Bots;

namespace Bnfour.TgBots.Interfaces.Factories;

/// <summary>
/// Provides a single bot with the given token, or null if no such bot found.
/// </summary>
public interface IBotFactory
{
    public IBot? GetBotByToken(string token);
}
