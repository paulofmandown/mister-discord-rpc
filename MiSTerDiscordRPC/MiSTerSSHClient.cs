using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace MiSTerDiscordRPC {
    public class MiSTerSSHClient {
        private string DefaultUser = "root";
        private string DefaultPass = "1";
        public string Address { get; set; }
        public PasswordConnectionInfo ConnectionInfo { get; set; }
        private SshClient Client = null;

        public  MiSTerSSHClient(string Address) {
            this.Address = Address;
            this.ConnectionInfo = new PasswordConnectionInfo(Address, DefaultUser, DefaultPass);
        }

        public MiSTerPresenceData GetMiSTerPresenceData() {
            var data = new MiSTerPresenceData();
            var processes = GetCoreProcesses();
            if (processes == null || processes == "") {
                return null;
            }
            data.Core = GetCoreName(processes);
            if (data.Core != "menu") {
                data.Rom = GetLatestRomName(processes);
            } else {
                data.Rom = "";
            }
            if (data.Rom == "~~~") {
                return GetSAMPresence();
            }
            return data;
        }

        public void Dispose() {
            if (Client != null && Client.IsConnected) {
                Client.Disconnect();
            }
        }

        private SshClient GetClient() {
            if (Client == null || !Client.IsConnected) {
                Client = new SshClient(ConnectionInfo);
                Client.Connect();
            }
            return Client;
        }

        private string GetCoreProcesses() {
            var processes = GetClient().CreateCommand("ps aux | grep [r]bf").Execute();
            if (processes == null || processes == "") {
                return null;
            }
            return processes;
        }

        private string GetCoreName(string processes) {
            var corename = new Stack<string>(processes.Split()[8].Split("/")).Pop().Replace(".rbf", "").Trim();
            if (corename.Contains("_")) {
                corename = corename.Split("_")[0];
            }
            return corename;
        }

        private string GetLatestRecentsFile() {
            string cmd = "ls -srt /media/fat/config/*recent* -1";
            Stack<string> filenames = new Stack<string>(GetClient().CreateCommand(cmd).Execute().Split('\n'));
            filenames.Pop();
            string filename = filenames.Pop();
            return filename;
        }

        private string GetLatestRomName(string processes) {
            var romname = "";
            if (processes.Contains(".mra")) {
                romname = new Stack<string>(processes.Split("/")).Pop().Replace(".mra", "").Trim();
            } else {
                var latestFile = GetLatestRecentsFile();
                if (!latestFile.Equals("")) {
                    romname = Path.GetFileNameWithoutExtension(GetClient().CreateCommand($"strings {latestFile}").Execute().Split('\n')[1]);
                }
            }
            return romname;
        }

        private MiSTerPresenceData GetSAMPresence() {
            var data = new MiSTerPresenceData();
            data.SuperAttractMode = true;
            data.Rom = GetClient().CreateCommand("cat /tmp/SAM_Game.txt").Execute().Trim();
            return data;
        }
    }
}
