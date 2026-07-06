# tg-bots-dotnet (deprecated)

>[!CAUTION]
>This project is no longer actively maintained and/or supported. Use at your own risk.
>
>I don't even run Cat macro bot anymore. Ladder bot works until it doesn't.

A suite of two small (mostly inline) (and _omega_ useful) bots for Telegram, written in .NET.

Initially a port and/or an upgrade of an earlier [Python version](https://github.com/bnfour/tg-bots), with a lot of things borrowed from my another .NET Telegram bot. Deprecated as mostly unused.

## Ladder bot
Inline bot that generates texts running along horizontal, vertical and diagonal directions simultaneously for extra expressiveness.

For example, prompting `sample text` to the bot generates buttons for two messages:
1. With spaces, the default:
```
S A M P L E   T E X T
A A
M   M
P     P
L       L
E         E

T             T
E               E
X                 X
T                   T
```

2. Without spaces, useful for longer strings:
```
SAMPLE TEXT
AA
M M
P  P
L   L
E    E

T      T
E       E
X        X
T         T
```

## Cat macro bot
Inline bot that can be used to post pictures searchable by defined captions. I've used it to store and post cat pictures I used to spam before I moved to Telegram and started spamming stickers. After some time, sticker spam won; I don't use these macro anymore.

>[!WARNING]
>This bot is known to stop working after some time (several months to years). Something is probably leaking.
>
>You can try to restart the app regularly to "mitigate" this issue (not that is a good fix).

### Usage
Querying the bot inline will provide prompts for images with somewhat matching captions from its database for quick posting.

This bot isn't strictly inline: administrator accounts (set in options) can manage image database via chat with the bot:
- Sending a captioned photo will add that photo and make it searchable by provided caption. This is default behavior, unless deletion mode is active — see below.
- `/delete` instructs bot to enter deletion mode (actually, anything starting with `/delet` will work).  
In this mode, the bot will try to match sent images with its database, and remove the matching one. The mode is active until a successful deletion, or manual cancellation via `/cancel`. For best results, use the inline query or forward prior bot output.
- `/cancel` will switch the bot out of deletion mode, if it's active.

This replicates the behavior of Python version. But this time, deletion by image actually works — there's no need to realize that after several years and implement deletion by caption as a stopgap!

## Configuration
App-specific options are defined in `SharedOptions` and `Options` sections in `appsettings.json`:
```jsonc
// ... omitted
"SharedOptions": {
    "WebhookUrl": "string"
},
"Options": {
    "LadderBotOptions": {
        "Token": "string" | null
    },
    "CatMacroBotOptions": {
        "Token": "string" | null,
        "Admins": [000000000, 000000000] // integer account IDs
    }
}
```

`WebhookUrl` is the URL to the app index as it appears to the outside world through the reverse proxy and _not_ the default localhost listening address. Webhooks to individual bots will be set to `[WebhookUrl]/[Token]`.

Subsections for individual bots follow next, each includes at least `Token` field. The Telegram API bot token goes here. If set to null, the bot is disabled.  
Cat macro bot also has an extra option — a list of accounts that can manage its database. The values here are integer account IDs, not usernames.

The rest of the file is generic ASP.NET Core config. Some points are interest are local listening URL (port) and the connection string for the Cat macro bot's database.

## Web frontend
There's also a simple index page that contains links to enabled bots, app version, and a link back to this repo. A (very cool) generic error page used in most of my web projects is included as well.

## Deployment
The app only listens on local address and has no SSL support, which is actually required for a bot backend. It is a deliberate choice to not handle this in the app.  
For running locally, [ngrok](https://ngrok.com/) is one of the options. For actual hosting, use any decent web server, like [nginx](https://nginx.org/), as a reverse SSL proxy.

## Version history
### [v1.0](https://github.com/bnfour/tg-bots-dotnet/tree/v1.0) — dotnet rewrite of a Python script
The initial release based on the [original Python version](https://github.com/bnfour/tg-bots).

The most notable feature is that the deletion by image for Cat macro bot actually worked this time.

### [v1.1](https://github.com/bnfour/tg-bots-dotnet/tree/v1.1) — added the architecture
Code quality improvement release.

The previous release had every service as a singleton, because I misunderstood [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)'s lifecycle when managing webhooks. This has been fixed. Alongside the lifecycle changes, there are also other architecture improvements, notably splitting the do-it-all "manager" class to single responsibility classes. There's even a bot factory now!

Notable changes from `v1.0`:
- .NET 8 runtime — an LTS one, probably will migrate straight to .NET 10 in the future
- Webhook URL in config is moved to its own subsection `SharedOptions` from `Options`
- Probably better architecture

### [v1.2](https://github.com/bnfour/tg-bots-dotnet/releases/tag/v1.2) — error pages
Small release (an actual release this time!) with a few features ported over from my another Telegram bot project.

The web part now has an (epic) error page to display on non-API GET request errors. The distinctive lack of a favicon is properly implemented now.

### [v1.2.1](https://github.com/bnfour/tg-bots-dotnet/releases/tag/v1.2.1) — good night sweet prince
A small refactoring-focused release before deprecation.

Migrated to .NET 10, updated NuGet references — the usual. Minor styling updates as well.

This is the last release for this repo.
