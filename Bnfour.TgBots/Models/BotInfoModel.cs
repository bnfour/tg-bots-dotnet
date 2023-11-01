namespace Bnfour.TgBots.Models;
/// <summary>
/// Info of the bot to display on the "landing" page.
/// </summary>
public class BotInfoModel
{
    /// <summary>
    /// Indicates if the bot is properly configured and active.
    /// </summary>
    public bool IsOnline { get; set; }
    /// <summary>
    /// If the bot is active, it's telegram username.
    /// Null if the bot is not active.
    /// </summary>
    public string? Username { get; set; }
}
