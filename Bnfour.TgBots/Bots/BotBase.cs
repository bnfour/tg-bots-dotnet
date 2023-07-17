using System;
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
    protected readonly TelegramBotClient? _client;

    private readonly string? _webhookUrl;

    protected abstract bool Inline { get; }

    public bool Enabled => _client != null && _webhookUrl != null;

    // TODO looks scuffed, any way to include the index URL in the options
    // while still declaring it once?
    public BotBase(string webhookIndex, BotOptionsBase options)
    {
        if (options.Token != null)
        {
            // don't care about the slash being or not being at the end of the URL
            _webhookUrl = webhookIndex.TrimEnd('/') + "/" + options.Token;
            _client = new TelegramBotClient(options.Token);
        }
    }

    private void ThrowIfNotEnabled()
    {
        if (!Enabled)
        {
            throw new ApplicationException("The bot is not enabled.");
        }
    }

    #region webhook manipulation

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

    public async Task RemoveWebhook()
    {
        ThrowIfNotEnabled();

        await _client!.DeleteWebhookAsync();
    }

    #endregion

    protected async Task Send(long accountId, string message)
    {
        ThrowIfNotEnabled();

        var chatId = new ChatId(accountId);

        await _client!.SendTextMessageAsync(chatId, message,
            parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

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

    protected abstract Task HandleInlineQuery(InlineQuery inlineQuery);

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

    protected async Task HandleCommand(long userId, string text)
    {
        var command = text.Split(' ', '\n').First().ToLowerInvariant();

        var commandHandled = await TryToFindAndRunCommand(command, userId, text);

        if (!commandHandled)
        {
            await ReplyToUnknownCommand(userId);
        }
    }

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
    // TODO make these strings changeable in descendants

    #region error replies

    protected async Task ReplyToArbitaryText(long userId)
    {
        await Send(userId, @"I did not quite catch that\.");
    }

    protected async Task ReplyToUnknownCommand(long userId)
    {
        await Send(userId, @"Unfortunately, this is not a command I recognize\. Try running /help command\.");
    }

    #endregion

    #region commands

    protected async Task HandleStartCommand(long userId)
    {
        await Send(userId, @"Welcome\!");
    }

    protected async Task HandleHelpCommand(long userId)
    {
        await Send(userId, @"Some kind of message about how to use this bot I guess\.");
    }

    protected async Task HandleAboutCommand(long userId)
    {
        await Send(userId, """
        bot\-name\-goes\-here version

        Part of mini bots suite\. [Open\-source\!](https://github.com/bnfour/tg-bots-dotnet)
        by bnfour, 2023\.
        """);
    }

    #endregion
}
