using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Interfaces.Services;
using Bnfour.TgBots.Options;
using Bnfour.TgBots.Options.BotOptions;
using Bnfour.TgBots.Services;

using Microsoft.EntityFrameworkCore;

// a shining example on how NOT to configure your app

var builder = WebApplication.CreateBuilder(args);
// we need to explicitly add NewtonsoftJson to parse webhook payloads
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddScoped<IBotInfoProviderService, BotInfoProviderService>();
builder.Services.AddScoped<IBotWebhookManagerService, BotWebhookManagerService>();
builder.Services.AddScoped<IUpdateHandlerService, UpdateHanderService>();

builder.Services.AddSingleton<ICatMacroBotAdminHelperService, CatMacroBotAdminHelperService>();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Options"));

// we're going to overengineer to be "futureproof"
// (and also to look even more like an enterprise-ready hello world app)

// this section exists just to enable me to pass
// a single options parameter into bot's constructors --
// we define a webhook url elsewhere (but once), and then copy the value
// to all the bot config classes we have (using reflection, on top of the things)
builder.Services.PostConfigure<ApplicationOptions>(appOptions =>
{
    // this section is only loaded once to populate the other one
    var sharedValue = builder.Configuration.GetSection("SharedOptions")
        .Get<SharedOptions>()?.WebhookUrl
            ?? throw new ApplicationException("SharedOptions not available");

    // i could just write two direct assignments to the properties
    // instead of all of this
    var optionsToInsert = appOptions.GetType().GetProperties()
        .Select(p => p.GetValue(appOptions))
        .Where(o => o != null && o.GetType().BaseType == typeof(BotOptionsBase))
        .Select(o => o as BotOptionsBase)
        .Where(o => o != null);
    
    foreach (var option in optionsToInsert)
    {
        // it is explicitly null-checked above
        option!.WebhookUrl = sharedValue;
    }
});

// this is used to force the code to use database from the current (e.g output for debug) directory
// instead of using the file in the project root every launch

// please note that this value ends with backslash, so in the connection string,
// file name goes straight after |DataDirectory|, no slashes of any kind
AppDomain.CurrentDomain.SetData("DataDirectory", AppContext.BaseDirectory);

// TODO move away from singletons everywhere?

builder.Services.AddDbContext<CatMacroBotContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatMacroBotConnectionString")),
    ServiceLifetime.Scoped, ServiceLifetime.Scoped);

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Lifetime.ApplicationStarted.Register(async () =>
    await (app.Services.GetService(typeof(IBotWebhookManagerService)) as IBotWebhookManagerService)!.SetWebhooks());

app.Lifetime.ApplicationStopped.Register(async () =>
    await (app.Services.GetService(typeof(IBotWebhookManagerService)) as IBotWebhookManagerService)!.RemoveWebhooks());

app.Run();
