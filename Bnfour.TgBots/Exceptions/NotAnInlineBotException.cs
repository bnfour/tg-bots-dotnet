namespace Bnfour.TgBots.Exceptions;

/// <summary>
/// Indicates that an inline request was sent to a bot that is not configured to accept inline queries.
/// The web server should return an error to the caller.
/// </summary>
public class NotAnInlineBotException : Exception { }
