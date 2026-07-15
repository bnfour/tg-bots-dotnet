namespace Bnfour.TgBots.Extensions;

public static class VersionExtensions
{
    /// <summary>
    /// Converts the app version as set in the project file to a user readable string.
    /// Build from the version is only displayed if not zero.
    /// </summary>
    public static string ToDisplayString(this Version version)
    {
        var result = $"{version?.Major ?? 0}.{version?.Minor ?? 0}{(version?.Build > 0 ? $".{version.Build}" : string.Empty)}";
#if DEBUG
        result += " debug";
#endif
        return result;
    }
}
