using Bnfour.TgBots.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Options"));

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
