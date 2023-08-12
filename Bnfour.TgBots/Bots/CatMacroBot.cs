using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Extensions;
using Bnfour.TgBots.Options.BotOptions;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Bots;

/// <summary>
/// Bot that stores a collectionof images searchable by their captions.
/// </summary>
public class CatMacroBot : BotBase
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webhookIndex">Common part of the webhook endpoint path, shared between bots.</param>
    /// <param name="options">Bot-specific options. Includes list of its admins.</param>
    /// <param name="context">Database context to use.</param>
    public CatMacroBot(string webhookIndex, CatMacroBotOptions options, CatMacroBotContext context)
        : base(webhookIndex, options)
    {
        _context = context;
        _admins = options.Admins ?? new List<long>();
    }

    /// <summary>
    /// Database context to use.
    /// </summary>
    private readonly CatMacroBotContext _context;

    /// <summary>
    /// List of admins able to modify the database.
    /// </summary>
    private readonly List<long> _admins;

    protected override bool Inline => true;

    // TODO don't forget to change after new and shiny reimplementation
    protected override string Name => "Cat macro bot (old)";

    protected override string HelpResponse => """
    Currently, I'll provide prompts for cat images from my collection. For example, try querying me for "stop posting".

    If you're my admin, you already know how to manage me.

    """.ToMarkdownV2()
    +
    """
    **Important note:** there are plans to completely redo this bot to make _media_ collections per user, rather than single global _image_ collection\. Stay tuned\. Or not\.
    """;

    protected override string StartResponse => """
    Hi there!

    I'm an inline bot, so feel free to summon me in other chats to post cat-related images.
    """.ToMarkdownV2();

    protected override async Task HandleInlineQuery(InlineQuery inlineQuery)
    {
        throw new NotImplementedException();
    }

    protected override async Task HandlePhoto(Message message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks whether the provided Telegram user ID is configured as the bot admin.
    /// </summary>
    /// <param name="id">ID to check.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    private bool IsAdmin(long id) => _admins.Contains(id);
}
