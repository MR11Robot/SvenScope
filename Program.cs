using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SvenScope
{
    public class ServerInfo
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }

    public class A2SInfo
    {
        public string ServerName { get; set; } = string.Empty;
        public string MapName { get; set; } = string.Empty;
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
    }

    public class A2SPlayer
    {
        public string Name { get; set; } = string.Empty;
        public float Duration { get; set; }
    }

    class Program
    {
        private const string SERVERS_FILE = "servers.json";
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                Clear();
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine("       SVEN CO-OP SERVER TRACKER");
                Console.WriteLine("═══════════════════════════════════════\n");
                Console.WriteLine("  1. Add new server");
                Console.WriteLine("  2. List saved servers");
                Console.WriteLine("  3. Delete server");
                Console.WriteLine("  4. Start tracking");
                Console.WriteLine("  5. Exit\n");
                Console.Write("  Your choice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddServer();
                        break;
                    case "2":
                        ListServers();
                        break;
                    case "3":
                        DeleteServer();
                        break;
                    case "4":
                        QueryServers();
                        break;
                    case "5":
                        Clear();
                        Console.WriteLine("\n  Exiting... Goodbye!\n");
                        return;
                    default:
                        Console.WriteLine("\n  [!] Invalid choice. Try again.");
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        static void Clear()
        {
            Console.Clear();
        }

        static List<ServerInfo> LoadServers()
        {
            if (!File.Exists(SERVERS_FILE))
                return [];

            string json = File.ReadAllText(SERVERS_FILE);
            return JsonSerializer.Deserialize<List<ServerInfo>>(json) ?? [];
        }

        static void SaveServers(List<ServerInfo> servers)
        {
            string json = JsonSerializer.Serialize(servers, _jsonOptions);
            File.WriteAllText(SERVERS_FILE, json);
        }

        static void AddServer()
        {
            Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("            ADD NEW SERVER");
            Console.WriteLine("═══════════════════════════════════════\n");
            Console.Write("  Enter server IP (or IP:PORT): ");
            string rawInput = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                Console.WriteLine("\n  [!] Server IP cannot be empty.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            string host;
            int port;

            if (rawInput.Contains(':'))
            {
                var parts = rawInput.Split(':', 2);
                host = parts[0].Trim();

                if (string.IsNullOrWhiteSpace(host))
                {
                    Console.WriteLine("\n  [!] Server IP cannot be empty.\n");
                    Console.WriteLine("  Press Enter to return to menu...");
                    Console.ReadLine();
                    return;
                }

                if (!int.TryParse(parts[1], out port) || port < 1 || port > 65535)
                {
                    Console.WriteLine("\n  [!] Invalid port number. Must be between 1 and 65535.\n");
                    Console.WriteLine("  Press Enter to return to menu...");
                    Console.ReadLine();
                    return;
                }
            }
            else
            {
                host = rawInput;
                Console.Write("  Enter server port (default 27015): ");
                string portInput = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(portInput))
                {
                    port = 27015;
                }
                else
                {
                    if (!int.TryParse(portInput, out port) || port < 1 || port > 65535)
                    {
                        Console.WriteLine("\n  [!] Invalid port number. Must be between 1 and 65535.\n");
                        Console.WriteLine("  Press Enter to return to menu...");
                        Console.ReadLine();
                        return;
                    }
                }
            }

            var servers = LoadServers();

            if (servers.Any(s => s.Host == host && s.Port == port))
            {
                Console.WriteLine($"\n  [!] Server {host}:{port} already exists.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            servers.Add(new ServerInfo { Host = host, Port = port });
            SaveServers(servers);

            Console.WriteLine($"\n  [+] Server added: {host}:{port}\n");
            Console.WriteLine("  Press Enter to return to menu...");
            Console.ReadLine();
        }

        static void ListServers()
        {
            Clear();
            var servers = LoadServers();

            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("            SAVED SERVERS");
            Console.WriteLine("═══════════════════════════════════════\n");

            if (servers.Count == 0)
            {
                Console.WriteLine("  [!] No servers added yet.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < servers.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {servers[i].Host}:{servers[i].Port}");
            }

            Console.WriteLine("\n  Press Enter to return to menu...");
            Console.ReadLine();
        }

        static void DeleteServer()
        {
            Clear();
            var servers = LoadServers();

            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("            DELETE SERVER");
            Console.WriteLine("═══════════════════════════════════════\n");

            if (servers.Count == 0)
            {
                Console.WriteLine("  [!] No servers added yet.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("  Saved servers:\n");
            for (int i = 0; i < servers.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {servers[i].Host}:{servers[i].Port}");
            }

            Console.Write("\n  Enter server number to delete (0 to cancel): ");
            string input = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("\n  [!] Invalid input.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            if (!int.TryParse(input, out int choice) || choice < 0 || choice > servers.Count)
            {
                Console.WriteLine("\n  [!] Invalid server number.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            if (choice == 0)
            {
                Console.WriteLine("\n  [!] Deletion cancelled.\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            var serverToDelete = servers[choice - 1];
            servers.RemoveAt(choice - 1);
            SaveServers(servers);

            Console.WriteLine($"\n  [+] Server deleted: {serverToDelete.Host}:{serverToDelete.Port}\n");
            Console.WriteLine("  Press Enter to return to menu...");
            Console.ReadLine();
        }

        static void QueryServers()
        {
            Clear();
            var servers = LoadServers();

            if (servers.Count == 0)
            {
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine("          SERVER TRACKING");
                Console.WriteLine("═══════════════════════════════════════\n");
                Console.WriteLine("  [!] No servers found. Add one first!\n");
                Console.WriteLine("  Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("          SERVER TRACKING");
            Console.WriteLine("═══════════════════════════════════════\n");

            foreach (var server in servers)
            {
                try
                {
                    var info = QueryServerInfo(server.Host, server.Port);
                    var players = QueryServerPlayers(server.Host, server.Port);

                    Console.WriteLine($"  Server: {server.Host}:{server.Port}");
                    Console.WriteLine($"  Name  : {info.ServerName}");
                    Console.WriteLine($"  Map   : {info.MapName}");
                    Console.WriteLine($"  Players: {players.Count}/{info.MaxPlayers}");

                    if (players.Count > 0)
                    {
                        Console.WriteLine();
                        for (int i = 0; i < players.Count; i++)
                        {
                            string name = string.IsNullOrEmpty(players[i].Name) ? "(Unnamed)" : players[i].Name;
                            Console.WriteLine($"    {i + 1:D2}. {name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("    No players online.");
                    }
                    Console.WriteLine("\n───────────────────────────────────────\n");
                }
                catch (SocketException)
                {
                    Console.WriteLine($"  [X] {server.Host}:{server.Port}");
                    Console.WriteLine($"      Timeout - Server not responding\n");
                    Console.WriteLine("───────────────────────────────────────\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [X] {server.Host}:{server.Port}");
                    Console.WriteLine($"      Error: {ex.Message}\n");
                    Console.WriteLine("───────────────────────────────────────\n");
                }
            }

            Console.WriteLine("  [+] Query completed.\n");
            Console.WriteLine("  Press Enter to return to menu...");
            Console.ReadLine();
        }

        static A2SInfo QueryServerInfo(string host, int port)
        {
            using var client = new UdpClient();
            client.Client.ReceiveTimeout = 3000;
            client.Connect(host, port);

            byte[] request = [0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00];
            client.Send(request, request.Length);

            IPEndPoint remoteEP = new(IPAddress.Any, 0);
            byte[] response = client.Receive(ref remoteEP);

            return ParseA2SInfo(response);
        }

        static A2SInfo ParseA2SInfo(byte[] data)
        {
            int index = 5;
            string serverName = ReadString(data, ref index);
            string mapName = ReadString(data, ref index);
            ReadString(data, ref index);
            ReadString(data, ref index);
            index += 2;
            byte players = data[index++];
            byte maxPlayers = data[index++];

            return new A2SInfo
            {
                ServerName = serverName,
                MapName = mapName,
                Players = players,
                MaxPlayers = maxPlayers
            };
        }

        static List<A2SPlayer> QueryServerPlayers(string host, int port)
        {
            using var client = new UdpClient();
            client.Client.ReceiveTimeout = 3000;
            client.Connect(host, port);

            byte[] request = [0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF];
            client.Send(request, request.Length);

            IPEndPoint remoteEP = new(IPAddress.Any, 0);
            byte[] response = client.Receive(ref remoteEP);

            if (response[4] == 0x41)
            {
                byte[] challenge = new byte[4];
                Array.Copy(response, 5, challenge, 0, 4);

                byte[] requestWithChallenge = [0xFF, 0xFF, 0xFF, 0xFF, 0x55, challenge[0], challenge[1], challenge[2], challenge[3]];
                client.Send(requestWithChallenge, requestWithChallenge.Length);
                response = client.Receive(ref remoteEP);
            }

            return ParseA2SPlayers(response);
        }

        static List<A2SPlayer> ParseA2SPlayers(byte[] data)
        {
            List<A2SPlayer> players = [];
            int index = 5;
            byte count = data[index++];

            for (int i = 0; i < count; i++)
            {
                index++;
                string name = ReadString(data, ref index);
                index += 4;
                float duration = BitConverter.ToSingle(data, index);
                index += 4;

                players.Add(new A2SPlayer { Name = name, Duration = duration });
            }

            return players;
        }

        static string ReadString(byte[] data, ref int index)
        {
            var sb = new StringBuilder();
            while (index < data.Length && data[index] != 0)
            {
                sb.Append((char)data[index]);
                index++;
            }
            index++;
            return sb.ToString();
        }
    }
}