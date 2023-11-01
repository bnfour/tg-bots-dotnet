namespace Bnfour.TgBots.Models;
/// <summary>
/// Model for the "landing" page. Only holds some info about the bots.
/// </summary>
public class HomeModel
{
    /// <summary>
    /// List of data models of every bot present in the app,
    /// whether or not it's set as an active bot.
    /// </summary>
    public required IEnumerable<BotInfoModel> Bots { get; set; }
}
