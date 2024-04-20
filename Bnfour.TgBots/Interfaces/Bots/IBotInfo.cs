using Bnfour.TgBots.Models;

namespace Bnfour.TgBots.Interfaces.Bots;

/// <summary>
/// Represents the bot from the status/landing page point of view.
/// </summary>
public interface IBotInfo
{
    /// <summary>
    /// Get a model to display on the web page.
    /// </summary>
    public Task<BotInfoModel> GetModel();
}
