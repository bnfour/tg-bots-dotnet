namespace Bnfour.TgBots.Exceptions;

/// <summary>
/// Indicates that some data critical for request processing was not supplied.
/// Message is used to indicate what exactly is missing for debug purposes.
/// The web server should return 400 Bad request.
/// </summary>
/// <remarks>
/// Sets the reason this was thrown, as in what is not provided.
/// </remarks>
/// <param name="message">Exception reason.</param>
public class NoRequiredDataException(string message) : Exception(message) { }
