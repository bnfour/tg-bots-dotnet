# tg-bots-dotnet
A suite of small (mostly inline) (omega useful) bots for Telegram, written in .NET.

Initially a port and/or an upgrade of earlier [Python version](https://github.com/bnfour/tg-bots), with a lot of things borrowed from my another .NET Telegram bot, so called [Dotnet Telegram forwarder](https://github.com/bnfour/dotnet-telegram-forwarder).

### Why rewrite?
TODO maybe include this section if I'll be able to formulate it clearly.

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
This bot isn't strictly inline: administrator accounts can manage pictures via chatting:
- Sending a captioned photo will add that photo and make it searchable by provided caption. This is default behavior, unless deletion mode is active -- see below.
- `/delete` instructs bot to enter deletion mode. (actually, anything starting with `/delet` will work)  
In this mode, the bot will try to match the sent images to its database, and remove the matching images. The mode is active until a successful deletion, or manual cancellation via `/cancel`. For best results, use the inline query or forward bot output.
- `/cancel` will switch the bot out of deletion mode, if it's active.


This mimics behavior of the very first Python version. But this time, deletion by the image actually works, so there's no need to wait several years to realize that and implement deletion by caption as a stopgap!

### New version
Current version is just a rewrite of 2019 Python version using the stack I'm more comfortable with. There are plans to:
- allow to store media other than images, searchable by type
- make media collections per user, accessible only by the user
- make registration limited similar to dotnet-telegram-forwarder

## TODO Configuration and deployment
Similar to dotnet-telegram-forwarder, I guess.
