using Bnfour.TgBots.Interfaces.Bots;

namespace Bnfour.TgBots.Interfaces.Factories;

/// <summary>
/// Provides bot info for all bots, to be displayed on the web page.
/// </summary>
public interface IBotInfoFactory
{
    public IEnumerable<IBotInfo> GetBotInfo();
}
