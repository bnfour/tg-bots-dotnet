using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Entities;
using Bnfour.TgBots.Enums;
using Bnfour.TgBots.Exceptions;
using Bnfour.TgBots.Extensions;
using Bnfour.TgBots.Options.BotOptions;
using Telegram.Bot.Types;

namespace Bnfour.TgBots.Bots;

/// <summary>
/// Bot that stores a collection of images searchable by their captions.
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
        
        var admins = options.Admins ?? new List<long>();
        _adminStatus = new();
        foreach (var admin in admins)
        {
            _adminStatus[admin] = CatMacroBotAdminStatus.Normal;
        }
    }

    /// <summary>
    /// Database context to use.
    /// </summary>
    private readonly CatMacroBotContext _context;

    // TODO move this to database 
    // so this can be stored outside of a singleton
    /// <summary>
    /// Currently enabled mode per admin account.
    /// </summary>
    private readonly Dictionary<long, CatMacroBotAdminStatus> _adminStatus;

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

        if (_adminStatus[fromId] == CatMacroBotAdminStatus.Normal)
        {
            await AddImage(message);
        }
        else if (_adminStatus[fromId] == CatMacroBotAdminStatus.Deletion)
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
            await Send(fromId, "Please provide a caption!".ToMarkdownV2());
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
        // TODO remove some data from the response?
        await Send(fromId, $"""
        OK! ('-^)b
        Guid: {newMacro.Id}
        FileId: {newMacro.FileId}
        FileUniqueId: {newMacro.FileUniqueId}
        """.ToMarkdownV2());
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
        
        _adminStatus[fromId] = CatMacroBotAdminStatus.Normal;

        await Send(fromId, "Removal OK! ('-^)b".ToMarkdownV2());
    }

    /// <summary>
    /// Checks whether the provided Telegram user ID is configured as the bot admin.
    /// </summary>
    /// <param name="id">ID to check.</param>
    /// <returns>True if the user is an admin, false otherwise.</returns>
    private bool IsAdmin(long id) => _adminStatus.ContainsKey(id);

    protected override async Task<bool> TryToFindAndRunCommand(string command, long userId, string fullText)
    {
        var handledByBase = await base.TryToFindAndRunCommand(command, userId, fullText);
        if (handledByBase)
        {
            return true;
        }
        // as with previous versions, anything like /delet, /delete, /delet_this
        // or even /delete_this_please_i_beg_of_you works
        if (command.StartsWith("/delet"))
        {
            await HandleDelete(userId);
            return true;
        }
        else if (command == "/cancel")
        {
            await HandleCancel(userId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handles the /delet* command. Switches the user to deletion mode,
    /// or reminds them it's already enabled.
    /// </summary>
    /// <param name="userId">ID of the user who sent the command to reply to.</param>
    private async Task HandleDelete(long userId)
    {
        if (_adminStatus[userId] == CatMacroBotAdminStatus.Deletion)
        {
            await Send(userId, "You're already in deletion mode!".ToMarkdownV2());
        }
        else
        {
            _adminStatus[userId] = CatMacroBotAdminStatus.Deletion;
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
        if (_adminStatus[userId] == CatMacroBotAdminStatus.Normal)
        {
            await Send(userId, "You've nothing to cancel!".ToMarkdownV2());
        }
        else
        {
            _adminStatus[userId] = CatMacroBotAdminStatus.Normal;
            await Send(userId, "Deletion mode cancelled! Image adding mode active!".ToMarkdownV2());
        }
    }
}
