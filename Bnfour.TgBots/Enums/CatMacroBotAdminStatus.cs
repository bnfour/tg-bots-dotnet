namespace Bnfour.TgBots.Enums;

/// <summary>
/// Used to switch between adding and removing pictures for an admin account.
/// </summary>
public enum CatMacroBotAdminStatus
{
    /// <summary>
    /// Default mode: try to add sent images to the database.
    /// </summary>
    Normal,
    /// <summary>
    /// Deletion mode: try to find sent images in the database and remove them.
    /// </summary>
    Deletion
}
