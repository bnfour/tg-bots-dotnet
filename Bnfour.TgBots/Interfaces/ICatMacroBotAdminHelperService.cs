using Bnfour.TgBots.Enums;

namespace Bnfour.TgBots.Interfaces;

/// <summary>
/// Holds the admin mode (whether the delete mode is active)
/// in-between requests, but outside the database.
/// (So, reset every app restart, should be no big deal)
/// </summary>
public interface ICatMacroBotAdminHelperService
{
    /// <summary>
    /// Holds the admin id => status data.
    /// </summary>
    Dictionary<long, CatMacroBotAdminStatus> AdminStatus { get; }
}
