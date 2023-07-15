var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "\"Hello World!\" from bnfour's Telegram bots, now on dotnet!");

app.Run();
