using DiscordRPC;
using System;

namespace MiSTerDiscordRPC {
    public class MiSTerPresenceData {
        public string Core { get; set; }
        public string Rom { get; set; }

        public RichPresence GetPresence() {
            return new RichPresence {
                Details = "Core - " + Core,
                State = Rom.Equals("") ? "Idling" : "Playing - " + Rom,
                Timestamps = new Timestamps(DateTime.UtcNow),
                Assets = new Assets()
                {
                    LargeImageKey = "mister-rpc",
                    LargeImageText = "MiSTer"
                }
            };
        }
    }
}
