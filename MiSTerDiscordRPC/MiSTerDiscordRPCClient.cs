using CommandLine;
using DiscordRPC;
using System;
using System.Threading;

namespace MiSTerDiscordRPC {

    class MiSTerDiscordRPCClient {
        private string Address { get; set; }
        private string LastCore { get; set; }
        private string LastRom { get; set; }
        private MiSTerSSHClient MiSTerClient { get; set; }
        private DiscordRpcClient DiscordClient { get; set; }
        private static readonly string DiscordAppId = "183817308228157440";

        private MiSTerDiscordRPCClient(Options o) {
            this.Address = o.Address;
        }

        private static void Main(string[] args) {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
                var rpcClient = new MiSTerDiscordRPCClient(o);
                rpcClient.Start();
                rpcClient.Run();
            });
        }

        private void Start() {
            if (Address == null || Address == "") {
                Address = GetInput("What is the MiSTer's Address?");
            }
            MiSTerClient = new MiSTerSSHClient(Address);
            DiscordClient = new DiscordRpcClient(DiscordAppId);
            DiscordClient.OnReady += (sender, e) => {};
            DiscordClient.OnPresenceUpdate += (sender, e) => {};
            try {
                DiscordClient.Initialize();
            } catch (Exception e) {
                Console.WriteLine($"Failed to connect to discord\nError: {e.Message}");
                return;
            }
        }

        private void Run() {
            try {
                while (true) {
                    var presenceData = MiSTerClient.GetMiSTerPresenceData();
                    if (presenceData == null || presenceData.Core == null || presenceData.Core == "") {
                        DiscordClient.ClearPresence();
                    }
                    UpdateDiscordPresence(presenceData);
                    Thread.Sleep(5000);
                }
            } catch (Exception e) {
                Console.WriteLine($"Runtime error: {e.Message}");
            } finally {
                DiscordClient.ClearPresence();
                DiscordClient.Dispose();
                MiSTerClient.Dispose();
            }
        }

        private string GetInput(string prompt) {
            Console.WriteLine(prompt);
            Console.ForegroundColor = ConsoleColor.Green;
            string Answer = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            return Answer;
        }

        private void UpdateDiscordPresence(MiSTerPresenceData data) {
            try {
                if (data.Rom != LastRom || data.Core != LastCore) {
                    Console.WriteLine($"Sending new presence: Playing {data.Rom} on {data.Core}");
                    DiscordClient.SetPresence(data.GetPresence());
                    LastRom = data.Rom;
                    LastCore = data.Core;
                }
            } catch (Exception e) {
                Console.WriteLine($"Failed to set presence\nError: {e.Message}");
                return;
            }
        }
    }

    class Options {
        [Option('h', "hostname",
            Default = null,
            HelpText = "The hostname (IP address) of the MiSTer",
            Required = false
        )]
        public string Address { get; set; }
    }
}
