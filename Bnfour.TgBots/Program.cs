var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "\"Hello World!\" from bnfour's Telegram bots, now on dotnet!");
app.MapControllers();

app.Run();
