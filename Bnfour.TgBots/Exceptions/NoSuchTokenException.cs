namespace Bnfour.TgBots.Exceptions;

/// <summary>
/// Indicates that there are no bots with a given token,
/// the web server should return 404 Not found
/// </summary>
public class NoSuchTokenException : Exception { }
