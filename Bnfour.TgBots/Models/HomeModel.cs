namespace Bnfour.TgBots.Models;
/// <summary>
/// Model for the "landing" page. Only holds some info about the bots.
/// </summary>
public class HomeModel
{
    public required IEnumerable<BotInfoModel> Bots { get; set; }
}
