using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Entities;
using Bnfour.TgBots.Enums;
using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Extensions;
using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Options.BotOptions;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bnfour.TgBots.Bots;

/// <summary>
/// Bot that stores a collection of images searchable by their captions.
/// </summary>
public class CatMacroBot : BotBase
{
    #region configuration

    // taken straight from Python version
    // TODO consider make MaxResults and SimilarityCutoff changeable via config file

    /// <summary>
    /// Maximum amount of images to return per query.
    /// </summary>
    private const int MaxResults = 7;

    /// <summary>
    /// Threshold for string matcher. Empirically set to provide "acceptable" results on my data.
    /// </summary>
    private const int SimilarityCutoff = 50;

    protected override bool Inline => true;

    // TODO don't forget to change after new and shiny reimplementation
    protected override string Name => "Cat macro bot (old)";

    protected override string HelpResponse => """
    Currently, I'll provide prompts for cat images from my collection. For example, try querying me for "stop posting".

    If you're my admin, you already know how to manage me.

    """.ToMarkdownV2()
    +
    // ToMarkdownV2 can't be used if the text also contains formatting as it'll just escape all of it,
    // so this is formatted by hand
    """
    **Important note:** there are plans to completely redo this bot to make _media_ collections per user, rather than single global _image_ collection\. Stay tuned\. Or not\.
    """;

    protected override string StartResponse => """
    Hi there!

    I'm an inline bot, so feel free to summon me in other chats to post cat-related images.
    """.ToMarkdownV2();

    #endregion

    /// <summary>
    /// Database context to use.
    /// </summary>
    private readonly CatMacroBotContext _context;

    /// <summary>
    /// Currently enabled mode per admin account.
    /// </summary>
    private readonly ICatMacroBotAdminHelperService _adminHelper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">Bot-specific options. Includes list of its admins.</param>
    /// <param name="context">Database context to use.</param>
    /// <param name="adminHelper">Helper service with persistent state to use.</param>
    public CatMacroBot(CatMacroBotOptions options, CatMacroBotContext context, ICatMacroBotAdminHelperService adminHelper)
        : base(options)
    {
        _context = context;
        _adminHelper = adminHelper;
    }

    protected override async Task HandleInlineQuery(InlineQuery inlineQuery)
    {
        // skips queries shorter than 3 characters
        // Telegram sends an empty query, so a bot can return default results,
        // but this bot is search only (for now?)
        if (!string.IsNullOrEmpty(inlineQuery.Query) && inlineQuery.Query.Length >= 3)
        {
            var results = GenerateResults(inlineQuery.Query);
            // TODO don't forget to update caching after the rewrite
            await _client!.AnswerInlineQueryAsync(inlineQuery.Id, results,
                cacheTime: 360, isPersonal: false);
        }
    }

    /// <summary>
    /// Generates the search results from the saved image database.
    /// </summary>
    /// <param name="input">String to query images by.</param>
    /// <returns>Up to <see cref="MaxResults"/> most matching results.</returns>
    private IEnumerable<InlineQueryResult> GenerateResults(string input)
    {
        // i tried to avoid materializing full image data in this method

        var captions = _context.Images.Select(i => i.Caption);

        // processing string to itself `(s) => s` is needed to support non-English characters,
        // as the default processor replaces anything matching [^ a-zA-Z0-9] with whitespaces
        var searchResults = FuzzySharp.Process.ExtractTop(input, captions, (s) => s,
            limit: MaxResults, cutoff: SimilarityCutoff)
            .Select(sr => sr.Value);

        return _context.Images
            .Where(i => searchResults.Contains(i.Caption))
            .Select(i => new InlineQueryResultCachedPhoto(i.Id.ToString(), i.FileId));
    }

    protected override async Task HandlePhoto(Message message)
    {
        var fromId = message.From!.Id;
        if (!IsAdmin(fromId))
        {
            await Send(fromId, "Access denied!".ToMarkdownV2());
            return;
        }
        // check if there is actually image data provided
        if (message.Photo == null || message.Photo.Length < 1)
        {
            throw new NoRequiredDataException("Message.Photo");
        }

        if (_adminHelper.AdminStatus[fromId] == CatMacroBotAdminStatus.Normal)
        {
            await AddImage(message);
        }
        else if (_adminHelper.AdminStatus[fromId] == CatMacroBotAdminStatus.Deletion)
        {
            await RemoveImage(message);
        }
    }

    /// <summary>
    /// Adds the image from the message to the database.
    /// This method does not check whether the sender has rights to add images,
    /// it needs to be done by the caller beforehand.
    /// </summary>
    /// <param name="message">Message with the image to add.</param>
    /// <exception cref="NoRequiredDataException">Thrown when there is no image IDs present.</exception>
    private async Task AddImage(Message message)
    {
        var fromId = message.From!.Id;
        
        // get the ids from the last Photo
        // (native version of the image and not the thumbnail)
        var photoSize = message.Photo!.Last();
        var fileId = photoSize.FileId;
        var fileUniqueId = photoSize.FileUniqueId;

        if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(fileUniqueId))
        {
            throw new NoRequiredDataException("Message.Photo.FileId and/or Message.Photo.FileUniqueId");
        }
        // duplicate checks
        if (_context.Images.Any(i => i.FileUniqueId == fileUniqueId))
        {
            var existingCaption = _context.Images.First(i => i.FileUniqueId == fileUniqueId).Caption;
            await Send(fromId, $"Error: duplicate image! Already saved as \"{existingCaption}\"".ToMarkdownV2());
            return;
        }
        if (string.IsNullOrEmpty(message.Caption))
        {
            var adviceFriend = string.IsNullOrEmpty(message.MediaGroupId)
                ? string.Empty
                : "\nProtip: send one image at a time.";

            await Send(fromId, ("Please provide a caption!" + adviceFriend).ToMarkdownV2());
            return;
        }
        if (_context.Images.Any(i => i.Caption == message.Caption))
        {
            await Send(fromId, $"Error: duplicate caption \"{message.Caption}\"!".ToMarkdownV2());
            return;
        }

        // if all the checks were passed, actually add a new image
        var newMacro = new CatMacro()
        {
            Caption = message.Caption,
            FileId = fileId,
            FileUniqueId = fileUniqueId
        };

        await _context.Images.AddAsync(newMacro);
        await _context.SaveChangesAsync();
        await Send(fromId, "OK! ('-^)b".ToMarkdownV2());
    }

    /// <summary>
    /// Tries to find and remove the image sent. If successful, exits deletion mode.
    /// Otherwise, prompts the user to try again.
    /// This method does not check whether the sender has rights to add images,
    /// it needs to be done by the caller beforehand.
    /// </summary>
    /// <param name="message">Message with the image to add.</param>
    /// <exception cref="NoRequiredDataException">Thrown when there is no image IDs present.</exception>
    private async Task RemoveImage(Message message)
    {
        var fromId = message.From!.Id;

        var photoSize = message.Photo!.Last();
        var fileUniqueId = photoSize.FileUniqueId;

        if (string.IsNullOrEmpty(fileUniqueId))
        {
            throw new NoRequiredDataException("Message.Photo.FileUniqueId");
        }

        var toRemove = _context.Images.SingleOrDefault(i => i.FileUniqueId == fileUniqueId);

        if (toRemove == null)
        {
            await Send(fromId, "Error! Image not found! Try again?".ToMarkdownV2());
            return;
        }

        _context.Remove(toRemove);
        await _context.SaveChangesAsync();
        
        _adminHelper.AdminStatus[fromId] = CatMacroBotAdminStatus.Normal;

        await Send(fromId, "Removal OK! ('-^)b".ToMarkdownV2());
    }

    /// <summary>
    /// Checks whether the provided Telegram user ID is configured as the bot admin.
    /// </summary>
    /// <param name="id">ID to check.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    private bool IsAdmin(long id) => _adminHelper.AdminStatus.ContainsKey(id);

    protected override async Task<bool> TryToFindAndRunCommand(string command, long userId, string fullText)
    {
        var handledByBase = await base.TryToFindAndRunCommand(command, userId, fullText);
        if (handledByBase)
        {
            return true;
        }
        // as with previous versions, anything like /delet, /delete, /delet_this
        // or even /delete_this_please_i_beg_of_you works
        if (IsAdmin(userId) && command.StartsWith("/delet"))
        {
            await HandleDelete(userId);
            return true;
        }
        else if (IsAdmin(userId) && command == "/cancel")
        {
            await HandleCancel(userId);
            return true;
        }
        // TODO not ghost non-admins by telling them that a perfectly valid command we just parsed does not exist?
        return false;
    }

    /// <summary>
    /// Handles the /delet* command. Switches the user to deletion mode,
    /// or reminds them it's already enabled.
    /// </summary>
    /// <param name="userId">ID of the user who sent the command to reply to.</param>
    private async Task HandleDelete(long userId)
    {
        if (_adminHelper.AdminStatus[userId] == CatMacroBotAdminStatus.Deletion)
        {
            await Send(userId, "You're already in deletion mode!".ToMarkdownV2());
        }
        else
        {
            _adminHelper.AdminStatus[userId] = CatMacroBotAdminStatus.Deletion;
            await Send(userId, "Deletion mode activated! Forward or query the image you want to remove, or /cancel".ToMarkdownV2());
        }
    }

    /// <summary>
    /// Handles the /cancel command. Switches the user to normal mode,
    /// or reminds them it's already enabled.
    /// </summary>
    /// <param name="userId">ID of the user who sent the command to reply to.</param>
    private async Task HandleCancel(long userId)
    {
        if (_adminHelper.AdminStatus[userId] == CatMacroBotAdminStatus.Normal)
        {
            await Send(userId, "You've nothing to cancel!".ToMarkdownV2());
        }
        else
        {
            _adminHelper.AdminStatus[userId] = CatMacroBotAdminStatus.Normal;
            await Send(userId, "Deletion mode cancelled! Image adding mode active!".ToMarkdownV2());
        }
    }
}
