using Bnfour.TgBots.Options.BotOptions;
using Telegram.Bot.Types;

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

    protected override string HelpResponse => throw new NotImplementedException();

    protected override string StartResponse => """
    Hi there!

    I'm an inline bot, so feel free to summon me in other chats to get prompts for ladder-like texts.
    """;

    protected override async Task<bool> TryToFindAndRunCommand(string command, long userId, string fullText)
    {
        var handledByBase = await base.TryToFindAndRunCommand(command, userId, fullText);
        if (handledByBase)
        {
            return true;
        }

        switch (command)
        {
            // TODO actual commands
            default:
                return false;
        }
    }

    protected override Task HandleInlineQuery(InlineQuery inlineQuery)
    {
        throw new NotImplementedException();
    }
}
