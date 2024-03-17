namespace Bnfour.TgBots.Options.BotOptions;

public class CatMacroBotOptions : BotOptionsBase
{
    /// <summary>
    /// List of user IDs that are considered the bot's admins,
    /// and are able to manage the database.
    /// </summary>
    public required List<long> Admins { get; set; }

    // please note that this bot configuration also includes
    // a connection string to its own little database
    // it's defined in "ConnectionStrings" section in appsettings.json
    // as "CatMacroBotConnectionString"
}
