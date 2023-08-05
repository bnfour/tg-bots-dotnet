using System.Text;
using Bnfour.TgBots.Extensions;
using Bnfour.TgBots.Options.BotOptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bnfour.TgBots.Bots;

/// <summary>
/// Bot that generates texts running along horizontal, vertical and diagonal directions.
/// </summary>
public class LadderBot : BotBase
{
    /// <summary>
    /// Constructor. Does nothing because there are no options unique to this bot.
    /// </summary>
    /// <param name="webhookIndex">Common part of the webhook endpoint path, shared between bots.</param>
    /// <param name="options">Bot-specific options.</param>
    public LadderBot(string webhookIndex, LadderBotOptions options) : base(webhookIndex, options) { }

    protected override bool Inline => true;

    protected override string Name => "Ladder bot";

    protected override string HelpResponse => """
    I'll provide two prompts for a query "sample text"
    One with spaces:
    ```
    S A M P L E   T E X T
    A A
    M   M
    P     P
    L       L
    E         E

    T             T
    E               E
    X                 X
    T                   T
    ```

    And the second one without spaces, which is useful for longer strings:
    ```
    SAMPLE TEXT
    AA
    M M
    P  P
    L   L
    E    E

    T      T
    E       E
    X        X
    T         T
    ```

    That's pretty much it\.
    """;

    protected override string StartResponse => """
    Hi there!

    I'm an inline bot, so feel free to summon me in other chats to get prompts for ladder-like texts.
    """.ToMarkdownV2();

    #region constants for generated results

    /// <summary>
    /// Title for a generated result with spaces.
    /// </summary>
    private const string SpacesTitle = "With spaces";

    /// <summary>
    /// Description for a generated result with spaces.
    /// </summary>
    private const string SpacesDescription = "Regular ladder text.";

    /// <summary>
    /// Relative URL of a thumbnail for a generated result with spaces.
    /// </summary>
    private const string SpacesThumbUrl = "/i/sparse.png";

    /// <summary>
    /// Title for a generated result without spaces.
    /// </summary>
    private const string NoSpacesTitle = "Compact without spaces";

    /// <summary>
    /// Description for a generated result without spaces.
    /// </summary>
    private const string NoSpacesDescription = "Uselful for long strings.";

    /// <summary>
    /// Relative URL of a thumbnail for a generated result without spaces.
    /// </summary>
    private const string NoSpacesThumbUrl = "/i/dense.png";

    /// <summary>
    /// Size of both sizes of both thumbnails, in pixels.
    /// </summary>
    private const int ThumbSize = 128;

    #endregion

    protected override async Task<bool> TryToFindAndRunCommand(string command, long userId, string fullText)
    {
        var handledByBase = await base.TryToFindAndRunCommand(command, userId, fullText);
        if (handledByBase)
        {
            return true;
        }
        // undocumented custom commands
        // (a way to test if this "framework" even works)
        switch (command)
        {
            case "/ping":
                await HandlePing(userId);
                return true;
            case "/pong":
                await HandlePong(userId);
                return true;
            default:
                return false;
        }
    }

    protected override async Task HandleInlineQuery(InlineQuery inlineQuery)
    {
        if (!string.IsNullOrEmpty(inlineQuery.Query) && inlineQuery.Query.Length > 0)
        {
            var results = GenerateResults(inlineQuery.Query);
            await _client!.AnswerInlineQueryAsync(inlineQuery.Id, results);
        }
    }

    /// <summary>
    /// Generates results for an inline query.
    /// </summary>
    /// <param name="input">Text from the query.</param>
    /// <returns>Results, ready to be sent to the Telegram API.</returns>
    private IEnumerable<InlineQueryResult> GenerateResults(string input)
    {
        input = Normalize(input);

        var spacesContent = new InputTextMessageContent(GenerateLadder(input, true))
        {
            ParseMode = ParseMode.MarkdownV2
        };
        var noSpacesContent = new InputTextMessageContent(GenerateLadder(input, false))
        {
            ParseMode = ParseMode.MarkdownV2
        };

        return new List<InlineQueryResult>
        {
            new InlineQueryResultArticle(Guid.NewGuid().ToString(), SpacesTitle, spacesContent)
            {
                Description = SpacesDescription,
                ThumbnailUrl = _webhookUrl!.TrimEnd('/') + SpacesThumbUrl,
                ThumbnailHeight = ThumbSize,
                ThumbnailWidth = ThumbSize
            },
            new InlineQueryResultArticle(Guid.NewGuid().ToString(), NoSpacesTitle, noSpacesContent)
            {
                Description = NoSpacesDescription,
                ThumbnailUrl = _webhookUrl!.TrimEnd('/') + NoSpacesThumbUrl,
                ThumbnailHeight = ThumbSize,
                ThumbnailWidth = ThumbSize
            }
        };
    }

    /// <summary>
    /// Sanitizes the user input by converting the user text into uppercase
    /// and removing some chars that may break ladder generation and/or formatting.
    /// </summary>
    /// <param name="input">String to convert.</param>
    /// <returns>String, safe to use in <see cref="GenerateLadder"/>.</returns>
    private string Normalize(string input)
    {
        input = input.Replace('\n', ' ').Replace('\r', ' ');
        input = input.Replace("`", "` ");
        return input.ToUpper();
    }

    /// <summary>
    /// Generates the ladder text, complete with Markdown formatting as a block of code.
    /// </summary>
    /// <param name="text">Text to generate from. Should be put through <see cref="Normalize"/> for best results.</param>
    /// <param name="withSpaces">Whether to insert spaces between letters.</param>
    /// <returns>Markdown-formatted string with a generated ladder text.</returns>
    private string GenerateLadder(string text, bool withSpaces)
    {
        var array = text.ToCharArray();

        var builder = new StringBuilder("```");
        builder.AppendLine();
        builder.AppendLine(string.Join(' ', array));
        for (int i = 1; i < array.Length; i++)
        {
            var c = array[i];
            var numberOfWhitespaces = withSpaces
                ? 2 * (i - 1) + 1
                : i - 1;
            builder.Append(c);
            builder.Append(new string(' ', numberOfWhitespaces));
            builder.Append(c);
            builder.AppendLine();
        }
        builder.Append("```");

        return builder.ToString();
    }

    /// <summary>
    /// Handles a /ping command be replying "pong".
    /// </summary>
    /// <param name="userId">User ID to send the reply to.</param>
    private async Task HandlePing(long userId)
    {
        await Send(userId, "pong");
    }

    /// <summary>
    /// Handles a /pong command be replying "ping".
    /// </summary>
    /// <param name="userId">User ID to send the reply to.</param>
    private async Task HandlePong(long userId)
    {
        await Send(userId, "ping");
    }
}
