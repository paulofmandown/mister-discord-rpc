using DiscordRPC;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MisterDiscordRPC {

    class Program {
        private static string DiscordAppId = "183817308228157440";
        private static string DefaultUser = "root";
        private static string DefaultPass = "1";

        private static void Main(string[] args) {
            string Address = GetInput("What is the MiSTer's Address?");
            var connectionInfo = new PasswordConnectionInfo(Address, DefaultUser, DefaultPass);
            using (var client = new SshClient(connectionInfo)) {
                client.Connect();
                var processes = client.CreateCommand("ps aux | grep [r]bf").Execute();
                var corename = new Stack<string>(processes.Split()[8].Split("/")).Pop().Replace(".rbf", "").Trim();
                if (corename.Contains("_")) {
                    corename = corename.Split("_")[0];
                }
                var romname = "";
                if (processes.Contains(".mra")) {
                    romname = new Stack<string>(processes.Split("/")).Pop().Replace(".mra", "").Trim();
                } else {
                    var latestFiles = new Stack<string>(client.CreateCommand("find /media/fat/config/ -mmin -10").Execute().Split('\n'));
                    latestFiles.Pop();
                    var latestFile = latestFiles.Pop();
                    if (!latestFile.Equals("")) {
                        romname = Path.GetFileNameWithoutExtension(client.CreateCommand($"strings {latestFile}").Execute().Split('\n')[2]);
                    }
                }
                UpdateDiscordPresence(corename, romname);
                client.Disconnect();
            }
        }

        private static string GetInput(string prompt) {
            Console.WriteLine(prompt);
            Console.ForegroundColor = ConsoleColor.Green;
            string Answer = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            return Answer;
        }

        private static void UpdateDiscordPresence(string corename, string romname) {
            var client = new DiscordRpcClient(DiscordAppId);
            client.OnReady += (sender, e) => {};
            client.OnPresenceUpdate += (sender, e) => {};
            try {
                client.Initialize();
            } catch (Exception e) {
                Console.WriteLine($"Failed to connect to discord\nError: {e.Message}");
                return;
            }
            try {
                client.SetPresence(new RichPresence{
                    Details = "Core - " + corename,
                    State = romname.Equals("") ? "Idling" : "Playing - " + romname,
                    Timestamps = new Timestamps(DateTime.UtcNow),
                    Assets = new Assets()
                    {
                        LargeImageKey = "mister-rpc",
                        LargeImageText = "MiSTer"
                    }
                });
            } catch (Exception e) {
                Console.WriteLine($"Failed to set presence\nError: {e.Message}");
                return;
            }
            Thread.Sleep(5000);
            client.ClearPresence();
            client.Dispose();
        }
    }
}
