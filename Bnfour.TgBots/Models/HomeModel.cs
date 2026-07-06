using System.Reflection;

using Bnfour.TgBots.Extensions;

namespace Bnfour.TgBots.Models;
/// <summary>
/// Model for the "landing" page.
/// </summary>
public class HomeModel
{
    /// <summary>
    /// List of data models of every bot present in the app,
    /// whether or not it's set as an active bot.
    /// </summary>
    public required IEnumerable<BotInfoModel> Bots { get; set; }

    public string Version =>
        Assembly.GetExecutingAssembly().GetName().Version?.ToDisplayString() ?? "no version";
}
