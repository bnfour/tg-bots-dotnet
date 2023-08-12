namespace Bnfour.TgBots.Entities;

/// <summary>
/// Represents a saved image with a caption.
/// </summary>
public class CatMacro
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Caption, under which the image is searchable.
    /// </summary>
    public required string Caption { get; set; }

    /// <summary>
    /// Media ID of the image.
    /// </summary>
    public required string MediaId { get; set; }
}
