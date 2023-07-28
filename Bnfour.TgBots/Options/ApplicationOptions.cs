using Bnfour.TgBots.Options.BotOptions;

namespace Bnfour.TgBots.Options;

/// <summary>
/// Holds application options.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// URL to application index to advertise to Telegram backend as a webhook base.
    /// The bot token will be appended to this for each enabled bot to form an unique URL.
    /// </summary>
    public required string WebhookUrl { get; set; }

    /// <summary>
    /// Options for the ladder bot.
    /// </summary>
    public required LadderBotOptions LadderBotOptions { get; set; }
}
