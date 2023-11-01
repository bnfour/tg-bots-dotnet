using Bnfour.TgBots.Models;

namespace Bnfour.TgBots.Interfaces;
/// <summary>
/// Used to provide status bot info to the "landing" page.
/// </summary>
public interface IBotInfoProviderService
{
    /// <summary>
    /// Get info about all registered bots to display.
    /// </summary>
    /// <returns>A list ready to be consumed by bot list partial. Needs to be wrapped into a full model.</returns>
    Task<IEnumerable<BotInfoModel>> GetInfo();
}
