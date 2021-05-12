# Discord Rich Presence for MiSTer

Tell Discord what you're doing on your MiSTer.

## Requirements

The MiSTer must be network-accessible via SSH.

The core must either use mra's or support the recents feature of the MiSTer. The application relies
on the active core's relevant recents file in `/media/fat/config/`.

## Running

You can run MiSTerDiscordRPC.exe by double-clicking or running from a cmd prompt.

When running from the cmd prompt you have the option to provide the MiSTer's hostname via argument:

```shell
MiSTerDiscordRPC.exe -h 10.0.0.2
```

If running outside of a prompt, or you do not provide the hostname via argument, you will be
prompted to provide the hostname at the application startup:

```shell
What is the MiSTer's Address?
10.0.0.2

```

## Notes / Known Issues

- I don't think this will work with MiSTer SAM, the attract mode suite. SAM works by using a fixed
  file name to link to the random rom to play. So it will likely always show that fixed name instead
  of the rom that is playing.

- If Discord closes while MiSTerDiscordRPC is running and re-opens, presence will be lost until a
  new Core or Rom is loaded or until MiSTerDiscordRPC is restarted. I'm open to suggestions on how
  to cleanly address that.

- No idea how this will behave for the computer cores. That's not really a feature I use a whole lot

## Shoutouts

- https://github.com/MechaDragonX/Bheithir

  - For an example of how to update Discord Rich Presence data from a C# application using
    https://github.com/Lachee/discord-rpc-csharp

- https://github.com/christopher-roelofs/misterobs

  - For inspiration on how get active core and rom information from the MiSTer
