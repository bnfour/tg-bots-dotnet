using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Options;
using Bnfour.TgBots.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// we want a single instance serving both interfaces
// it is a singleton because it has to manage webhooks among other things
// this is not an ideal solution, as it's now possible to inject and use the BotManagerService directly
// and not via intended separated interfaces, but i really wanted the interfaces separated
// of course, it's also possible to split BotManagerService to two classes serving a common bot list,
// but i can't be bothered to to that ¯\_(ツ)_/¯
builder.Services.AddSingleton<BotManagerService>();

builder.Services.AddSingleton<IBotManagerService>(s => s.GetService<BotManagerService>()!);
builder.Services.AddSingleton<IBotInfoProviderService>(s => s.GetService<BotManagerService>()!);

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Options"));

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Lifetime.ApplicationStarted.Register(async () =>
    await (app.Services.GetService(typeof(IBotManagerService)) as IBotManagerService)!.SetWebhooks());

app.Lifetime.ApplicationStopped.Register(async () =>
    await (app.Services.GetService(typeof(IBotManagerService)) as IBotManagerService)!.RemoveWebhooks());

app.Run();
