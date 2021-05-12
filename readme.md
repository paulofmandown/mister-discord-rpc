# Discord Rich Presence for MiSTer

Tell Discord what you're doing on your MiSTer.

## Requirements

MiSTer must be on the same network as the PC running Discord and mdrpc. The MiSTer must also be
accessible via SSH.

The core must support the recents feature of the MiSTer. The application relies on the
`/tmp/CORENAME` file and the core's relevant recents file in `/media/fat/config/`.
