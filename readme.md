# tg-bots-dotnet
A suite of small (mostly inline) (omega useful) bots for Telegram, written in .NET 7.

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

## TODO Cat macro bot
Probably going to change to allow media other than images and maybe some other features.

## TODO Configuration and deployment
Similar to dotnet-telegram-forwarder, I guess.