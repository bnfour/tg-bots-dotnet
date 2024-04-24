using Bnfour.TgBots.Options.BotOptions;

namespace Bnfour.TgBots.Options;

/// <summary>
/// Holds application options.
/// Each subsection contains options for a single bot class.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Options for the ladder bot.
    /// </summary>
    public required LadderBotOptions LadderBotOptions { get; set; }

    /// <summary>
    /// Options for the cat macro bot.
    /// </summary>
    public required CatMacroBotOptions CatMacroBotOptions { get; set; }
}
