namespace Bnfour.TgBots.Models;

/// <summary>
/// Represents the data to be shown on the error page.
/// </summary>
/// <param name="statusCode">Status code to be represented.</param>
public class ErrorModel(int statusCode)
{
    /// <summary>
    /// A list of descriptions for supported error codes.
    /// </summary>
    private static readonly Dictionary<int, string> _codeDescriptions = new()
    {
        [404] = "Not found",
        [405] = "Method not allowed",
        [500] = "Internal server error",
        // an easter egg
        [687] = "Well, you found me. Congratulations. Was it worth it?"
    };

    /// <summary>
    /// Status code for the error page.
    /// </summary>
    public int StatusCode { get; set; } = statusCode;
    
    /// <summary>
    /// Human-readable representation of the error.
    /// </summary>
    public string Description => _codeDescriptions.TryGetValue(StatusCode, out string? value) ? value : "Unknown error";
}