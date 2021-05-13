using DiscordRPC;
using System;

namespace MiSTerDiscordRPC {
    public class MiSTerPresenceData {
        public string Core { get; set; }
        public string Rom { get; set; }
        public bool SuperAttractMode { get; set; } = false;

        public RichPresence GetPresence() {
            string state;
            if (!SuperAttractMode) {
                state = Rom.Equals("") ? "Idling" : "Playing - " + Rom;
            } else {
                state = Rom;
            }

            return new RichPresence {
                Details = SuperAttractMode ? "Super Attract Mode" : "Core - " + Core,
                State = state,
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
