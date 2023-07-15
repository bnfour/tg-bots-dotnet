var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "\"Hello World!\" from bnfour's Telegram bots, now on dotnet!");
app.UseStaticFiles(new StaticFileOptions
{
    // only images are currently expected to being served as static files
    RequestPath = "/i",
});

app.Run();
