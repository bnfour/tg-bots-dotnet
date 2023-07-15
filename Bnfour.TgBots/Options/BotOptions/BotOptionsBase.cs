namespace Bnfour.TgBots.Options.BotOptions;

/// <summary>
/// Base class for options common to all bots.
/// </summary>
public abstract class BotOptionsBase
{
    /// <summary>
    /// Telegram bot token. If set to null, the bot is disabled.
    /// </summary>
    public string? Token { get; set; }
}
