namespace Bnfour.TgBots.Exceptions;

/// <summary>
/// Indicates that some data critical for request processing was not supplied.
/// Message is used to indicate what exactly is missing for debug purposes.
/// The web server should return 400 Bad request.
/// </summary>
public class NoRequiredDataException: Exception
{
    /// <summary>
    /// Sets the reason this was thrown, as in what is not provided.
    /// </summary>
    /// <param name="message">Exception reason.</param>
    public NoRequiredDataException(string message) : base(message) { }
}
