using Bnfour.TgBots.Contexts;
using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Options;
using Bnfour.TgBots.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

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

// this is used to force the code to use database from the current (e.g output for debug) directory
// instead of using the file in the project root every launch

// please note that this value ends with backslash, so in the connection string,
// file name goes straight after |DataDirectory|, no slashes of any kind
AppDomain.CurrentDomain.SetData("DataDirectory", AppContext.BaseDirectory);

// TODO move away from singletons everywhere?
// probably move bots TelegramBotClient instances to a singleton
// that manages the webhooks and holds the instances to use in other services,
// which may be made scoped then (are they?)

// or just yolo and make evertything scoped, starting from BotManagerService
// and see what happens
builder.Services.AddDbContext<CatMacroBotContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatMacroBotConnectionString")),
    ServiceLifetime.Singleton, ServiceLifetime.Singleton);

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
