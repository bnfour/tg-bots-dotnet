namespace Bnfour.TgBots.Extensions;

// see https://core.telegram.org/bots/api#markdownv2-style
// for details about so-called Markdown V2 formatting

/// <summary>
/// Helper to make strings MarkdownV2 friendly. Used when sending,
/// so most strings in the app can be written without care for formatting.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// List of characters to escape in order to satisfy the parser.
    /// </summary>
    private readonly static string[] _toEscape =
    [
        "_", "*", "[", "]", "(", ")", "~", "`", ">",
        "#", "+", "-", "=", "|", "{", "}", ".", "!"
    ];

    /// <summary>
    /// Escapes the dangerous symbols in the string.
    /// </summary>
    /// <param name="s">String to process.</param>
    /// <returns>Telegram Markdown v2 friendly string.</returns>
    public static string ToMarkdownV2(this string s)
    {
        foreach (var c in _toEscape)
        {
            s = s.Replace(c, @"\" + c);
        }
        return s;
    }
}
