using Bnfour.TgBots.Models;
using Bnfour.TgBots.Options.BotOptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bnfour.TgBots.Bots;

/// <summary>
/// Common class for a bot.
/// </summary>
public abstract class BotBase
{
    /// <summary>
    /// Telegram bot client instance to use.
    /// </summary>
    protected readonly TelegramBotClient? _client;

    /// <summary>
    /// URL to be used as webhook endpoint. If null, the bot is considered disabled.
    /// </summary>
    private readonly string? _webhookUrl;

    /// <summary>
    /// Bot's token. Used to verify update origin.
    /// </summary>
    private readonly string? _token;

    /// <summary>
    /// Indicates if the bot is inline and is able to handle inline queries.
    /// </summary>
    protected abstract bool Inline { get; }

    /// <summary>
    /// If set to false, the bot is not available and will throw on API usage.
    /// Checking for this is supposed to be caller's responsibility.
    /// </summary>
    public bool Enabled => _client != null || _webhookUrl != null;

    // TODO looks scuffed, any way to include the index URL in the options
    // while still declaring it once?

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webhookIndex">Common part of the webhook endpoint path, shared between bots.</param>
    /// <param name="options">Bot-specific options.</param>
    public BotBase(string webhookIndex, BotOptionsBase options)
    {
        if (!string.IsNullOrEmpty(options.Token))
        {
            // don't care about the slash being or not being at the end of the URL
            _webhookUrl = webhookIndex.TrimEnd('/') + "/" + options.Token;
            _client = new TelegramBotClient(options.Token);
            _token = options.Token;
        }
    }

    /// <summary>
    /// Checks whether passed string is an actual bot token to verify the request actully comes from Telegram backend.
    /// </summary>
    /// <param name="givenToken">Token received from request.</param>
    /// <returns>True for actual token, false otherwise.</returns>
    public bool IsToken(string givenToken)
    {
        return givenToken.Equals(_token);
    }

    /// <summary>
    /// Checks if the configuration is provided. If not, throws.
    /// </summary>
    private void ThrowIfNotEnabled()
    {
        if (!Enabled)
        {
            throw new ApplicationException("The bot is not enabled.");
        }
    }

    #region webhook manipulation

    /// <summary>
    /// Sets the webhook.
    /// </summary>
    public async Task SetWebhook()
    {
        ThrowIfNotEnabled();

        var allowedUpdateTypes = new[] { UpdateType.Message };
        if (Inline)
        {
            allowedUpdateTypes.Append(UpdateType.InlineQuery);
        }
        
        await _client!.SetWebhookAsync(_webhookUrl!, allowedUpdates: allowedUpdateTypes);
        
    }

    /// <summary>
    /// Removes the webhook.
    /// </summary>
    public async Task RemoveWebhook()
    {
        ThrowIfNotEnabled();

        await _client!.DeleteWebhookAsync();
    }

    #endregion

    /// <summary>
    /// Sends a message, formated in MarkdownV2. See https://core.telegram.org/bots/api#markdownv2-style
    /// </summary>
    /// <param name="accountId">ID of the user to send message to.</param>
    /// <param name="message">Message to send, in MarkdownV2.</param>
    protected async Task Send(long accountId, string message)
    {
        ThrowIfNotEnabled();

        var chatId = new ChatId(accountId);

        await _client!.SendTextMessageAsync(chatId, message,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    /// <summary>
    /// Public entry point for handling messages from Telegram backend.
    /// </summary>
    /// <param name="update">Update to process.</param>
    public async Task HandleUpdate(Update update)
    {
        ThrowIfNotEnabled();

        if (update.InlineQuery != null)
        {
            if (Inline)
            {
                await HandleInlineQuery(update.InlineQuery);
            }
            else
            {
                // TODO throw a custom exception here so we can return a 4xx code instead of 5xx
                throw new NotImplementedException("This bot does not support inline queries");
            }
        }
        else if (update.Message != null)
        {
            await HandleMessage(update.Message);
        }
        else
        {
            throw new ApplicationException("Unsupported message type we did't subscribe for, or empty message and inline query.");
        }
    }

    /// <summary>
    /// Handles a Message, which is anything sent to bot directly via chat: text, image, sticker etc.
    /// </summary>
    /// <param name="message">Message to process.</param>
    protected async Task HandleMessage(Message message)
    {
        if (message.From == null)
        {
            throw new ApplicationException("No user in message");
        }

        switch (message.Type)
        {
            case MessageType.Text:
                await HandleText(message);
                break;
            // TODO other message types
            default:
                await ReplyToArbitaryText(message.From.Id);
                break;
        }
    }

    /// <summary>
    /// Handles an inline query, if the bot is designated as an inline bot.
    /// </summary>
    /// <param name="inlineQuery">Inline query to process.</param>
    protected abstract Task HandleInlineQuery(InlineQuery inlineQuery);

    /// <summary>
    /// Handles text message.
    /// </summary>
    /// <param name="message">Message to process. Should be of <see cref="Message.Text"/> type and contain text.</param>
    protected async Task HandleText(Message message)
    {
        var text = message.Text ?? String.Empty;

        if (text.StartsWith("/"))
        {
            await HandleCommand(message.From!.Id, text);
        }
        else
        {
            // From is null-checked before calling this method
            await ReplyToArbitaryText(message.From!.Id);
        }
    }

    /// <summary>
    /// Handles commands (actually, text starting with "/") sent by users.
    /// If a matching command is found, it is run, otherwise an error message is sent.
    /// </summary>
    /// <param name="userId">User that sent the command.</param>
    /// <param name="text">Full text of the command.</param>
    protected async Task HandleCommand(long userId, string text)
    {
        var command = text.Trim().Split(' ', '\n').First().ToLowerInvariant();

        var commandHandled = await TryToFindAndRunCommand(command, userId, text);

        if (!commandHandled)
        {
            await ReplyToUnknownCommand(userId);
        }
    }

    /// <summary>
    /// Tries to find and run a command by its name.
    /// </summary>
    /// <param name="command">Actual command, text from starting slash up to the first whitespace.</param>
    /// <param name="userId">User that sent the command.</param>
    /// <param name="fullText">Full text of the command, in case arguments matter.</param>
    /// <returns>True if command was found and executed, false otherwise.</returns>
    protected async Task<bool> TryToFindAndRunCommand(string command, long userId, string fullText)
    {
        switch (command)
        {
            case "/start":
                await HandleStartCommand(userId);
                return true;
            case "/help":
                await HandleHelpCommand(userId);
                return true;
            case "/about":
                await HandleAboutCommand(userId);
                return true;
            default:
                return false;
        }
    }

    // note to self: don't forget the replies are in MarkdownV2
    // TODO make these strings changeable in descendants, finalize the wordings
    // maybe some variables for bot instances to fill, like its name?

    #region error replies

    /// <summary>
    /// Sends a message indicating the text (or other unsupported message type) was not recognized (it's not a command at all).
    /// </summary>
    /// <param name="userId">ID of the user to send the message to.</param>
    protected async Task ReplyToArbitaryText(long userId)
    {
        await Send(userId, @"I did not quite catch that\.");
    }

    /// <summary>
    /// Sends a message indicating the text was recognized as a command, but there is no such command available.
    /// </summary>
    /// <param name="userId">ID of the user to send the message to.</param>
    protected async Task ReplyToUnknownCommand(long userId)
    {
        await Send(userId, @"Unfortunately, this is not a command I recognize\. Try running /help command\.");
    }

    #endregion

    #region commands

    /// <summary>
    /// Handles /start command by sending a welcome message to the user.
    /// </summary>
    /// <param name="userId">ID of the user to send the message to.</param>
    protected async Task HandleStartCommand(long userId)
    {
        await Send(userId, @"Welcome\!");
    }

    /// <summary>
    /// Handles /help command by sending a usage help message to the user.
    /// </summary>
    /// <param name="userId">ID of the user to send the message to.</param>
    protected async Task HandleHelpCommand(long userId)
    {
        await Send(userId, @"Some kind of message about how to use this bot I guess\.");
    }

    /// <summary>
    /// Handles /about command by sending bot info: name, version, and repo link.
    /// </summary>
    /// <param name="userId">ID of the user to send the message to.</param>
    protected async Task HandleAboutCommand(long userId)
    {
        await Send(userId, """
        bot\-name\-goes\-here version

        Part of mini bots suite\. [Open\-source\!](https://github.com/bnfour/tg-bots-dotnet)
        by bnfour, 2023\.
        """);
    }

    #endregion

    /// <summary>
    /// Gets bot info for the "landing" page.
    /// </summary>
    /// <returns>Bot info for the "landing" page.</returns>
    public async Task<BotInfoModel> GetModel()
    {
        return new BotInfoModel
        {
            IsOnline = Enabled,
            Username = Enabled ? (await _client!.GetMeAsync()).Username : null
        };
    }
}
