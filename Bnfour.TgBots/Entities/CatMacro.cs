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

    // see https://core.telegram.org/bots/api#photosize

    /// <summary>
    /// "file_id" of the image. Used to generate inline replies.
    /// Is not stable and changes in every new message with the same image,
    /// but is _the_ way to download or reuse the image.
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// "file_unique_id" of the image. Used to identify the images sent to the bot
    /// for deletion.
    /// </summary>
    public required string FileUniqueId { get; set; }
}
