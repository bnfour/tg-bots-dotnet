namespace Bnfour.TgBots.Options;

/// <summary>
/// Holds the options that will be made available to all subsections defined
/// in <see cref="ApplicationOptions"/>. The same values will be supplied via
/// post-configuration of the main options.
/// The values are here to be defined only once.
/// </summary>
public class SharedOptions
{
    /// <summary>
    /// URL to application index to advertise to Telegram backend as a webhook base.
    /// The bot token will be appended to this for each enabled bot to form an unique URL.
    /// </summary>
    public required string WebhookUrl { get; set; }
}
