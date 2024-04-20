using Microsoft.Extensions.Options;

using Bnfour.TgBots.Enums;
using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Options;

namespace Bnfour.TgBots.Services;

public class CatMacroBotAdminHelperService : ICatMacroBotAdminHelperService
{
    private readonly Dictionary<long, CatMacroBotAdminStatus> _status = [];

    public Dictionary<long, CatMacroBotAdminStatus> AdminStatus => _status;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">Options to get the list of admins from.</param>
    public CatMacroBotAdminHelperService(IOptions<ApplicationOptions> options)
    {
        foreach (var adminId in options.Value?.CatMacroBotOptions?.Admins ?? [ ])
        {
            _status[adminId] = CatMacroBotAdminStatus.Normal;
        }
    }
}
