using Renci.SshNet;
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
            if (string.IsNullOrEmpty(processes)) {
                return null;
            }
            data.Core = GetCoreName(processes);
            if (data.Core != "menu") {
                data.Rom = GetLatestRomName(processes);
            } else {
                data.Rom = string.Empty;
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
            if (string.IsNullOrEmpty(processes)) {
                return null;
            }
            return processes;
        }

        private string GetCoreName(string processes) {
            var corename = "";
            var processStack = new Stack<string>(processes.Split()[8].Split("/"));
            if (processStack.Count > 0) {
                corename = processStack.Pop().Replace(".rbf", "").Trim();
                if (corename.Contains("_")) {
                    corename = corename.Split("_")[0];
                }
            }
            return corename;
        }

        private string GetLatestRecentsFile() {
            var filename = "";
            string cmd = "ls -srt /media/fat/config/*recent* -1";
            Stack<string> filenames = new Stack<string>(GetClient().CreateCommand(cmd).Execute().Split('\n'));
            if (filenames.Count > 0) {
                filenames.Pop();
                filename = filenames.Pop();
            }
            return filename;
        }

        private string GetLatestRomName(string processes) {
            var romname = string.Empty;
            if (processes.Contains(".mra")) {
                var processStack = new Stack<string>(processes.Split("/"));
                if (processStack.Count > 0) {
                    romname = processStack.Pop().Replace(".mra", "").Trim();
                }
            } else {
                var latestFile = GetLatestRecentsFile();
                if (latestFile == null || !string.IsNullOrEmpty(latestFile)) {
                    romname = Path.GetFileNameWithoutExtension(GetClient().CreateCommand($"strings {latestFile}").Execute().Split('\n')[2]);
                }
            }
            return romname;
        }
    }
}
