# tg-bots-dotnet
A suite of small (mostly inline) (and omega useful) bots for Telegram, written in .NET.

Initially a port and/or an upgrade of an earlier [Python version](https://github.com/bnfour/tg-bots), with a lot of things borrowed from my another .NET Telegram bot, so called [Dotnet Telegram forwarder](https://github.com/bnfour/dotnet-telegram-forwarder). More updates to come?

There's also a very simple status page that shows links to active bots.

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
Inline bot that can be used to post pictures searchable by defined captions. I use it to store and post cat pictures I used to spam before I moved to Telegram and started spamming stickers.

### Usage
Querying the bot inline will provide prompts for images with somewhat matching captions from its database for quick posting.

This bot isn't strictly inline: administrator accounts can manage image database via chat with the bot:
- Sending a captioned photo will add that photo and make it searchable by provided caption. This is default behavior, unless deletion mode is active -- see below.
- `/delete` instructs bot to enter deletion mode (actually, anything starting with `/delet` will work).  
In this mode, the bot will try to match the sent images to its database, and remove the matching images. The mode is active until a successful deletion, or manual cancellation via `/cancel`. For best results, use the inline query or forward bot output.
- `/cancel` will switch the bot out of deletion mode, if it's active.


This mimics behavior of the very first Python version. But this time, deletion by the image actually works, so there's no need to wait several years to realize that and implement deletion by caption as a stopgap!

## Configuration
Settings for the app are defined in `SharedOptions` and `Options` sections in `appsettings.json`:
```jsonc
// ... omitted
"SharedOptions": {
    "WebhookUrl": "string"
},
"Options": {
    "LadderBotOptions": {
        "Token": "string or null"
    },
    "CatMacroBotOptions": {
        "Token": "string or null",
        "Admins": [000000000, 000000000] // integer account IDs
    }
}
```

`WebhookUrl` is the URL to the app index as it appears to the outside world through the reverse proxy and _not_ the default localhost listening address. Webhooks to individual bots will be set to `[WebhookUrl]/[Token]`.

Subsections for individual bots follow next, each includes at least `Token` field. The Telegram API bot token goes here. If set to null, the bot is disabled.  
Cat macro bot also has an extra option -- a list of accounts that can manage its database. The values here are integer IDs, not usernames.

The rest of the file is generic ASP.NET Core config. Some points are interest are local listening URL (port) and the connection string for the Cat macro bot.

## Deployment
As briefly mentioned in previous section, the app only listens on local address and has no SSL support, which is actually required for a bot backend. It is a deliberate choice to not handle this in the app.  
For running locally, [ngrok](https://ngrok.com/) is one of the options. For actual hosting, use any decent web server as a reverse proxy; I'm using [nginx](https://nginx.org/) for that.

## Futher developments
Very tentative.

### New version (and name) for Cat macro bot
Current version is just a rewrite of Python version. There are plans to:
- allow to store media other than images, searchable by type as well
- make media collections per user, accessible only by the user
- make user registration limited similar to dotnet-telegram-forwarder
- rename it to something new to signify it can do more that (cat) pictures now

### New bot(s) (?)
This whole "take a _mostly_ working app and make it from scratch one more time on another stack" started because I was tinkering with original version to make another bot to use in my chats. While doing that, I thought that the architecture could be better and so decided to make a .NET version as I'm more comfortable with it.  

There's no new bots, but we're getting there, at least the architecture was fixed ╭( ･ㅂ･)و

## Version history
### [v1.0](https://github.com/bnfour/tg-bots-dotnet/tree/v1.0) — dotnet rewrite of a Python script
The initial release based on the [original Python version](https://github.com/bnfour/tg-bots).

The most notable feature is that the deletion by image for Cat macro bot actually worked this time.

### v1.1 — added the architecture
Code quality improvement release.

The previous release had every service as a singleton, because I misunderstood [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)'s lifecycle when managing webhooks. This has been fixed. Alongside the lifecycle changes, there are also other architecture improvements, notably splitting the do-it-all "manager" class to single responsibility classes. There's even a bot factory now!

Notable changes from `v1.0`:
- .NET 8 runtime — an LTS one, probably will migrate straight to .NET 10 in the future
- Webhook URL in config is moved to its own subsection `SharedOptions` from `Options`
- Probably better architecture
