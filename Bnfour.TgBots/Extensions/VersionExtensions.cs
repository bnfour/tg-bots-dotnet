namespace Bnfour.TgBots.Extensions;

public static class VersionExtensions
{
    public static string ToDisplayString(this Version version)
    {
        var result = $"{version?.Major ?? 0}.{version?.Minor ?? 0}{(version?.Build > 0 ? $".{version.Build}" : string.Empty)}";
#if DEBUG
        result += " debug";
#endif
        return result;
    }
}
