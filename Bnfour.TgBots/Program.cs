using Bnfour.TgBots.Interfaces;
using Bnfour.TgBots.Options;
using Bnfour.TgBots.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IBotManagerService, BotManagerService>();

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
