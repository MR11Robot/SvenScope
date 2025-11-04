import json
import os
import socket
import a2s
from InquirerPy import inquirer

SERVERS_FILE = "servers.json"

def load_servers():
    if not os.path.exists(SERVERS_FILE):
        return []
    with open(SERVERS_FILE, "r") as f:
        return json.load(f)

def save_servers(servers):
    with open(SERVERS_FILE, "w") as f:
        json.dump(servers, f, indent=4)

def clear():
    os.system('cls' if os.name == 'nt' else 'clear')

def add_server():
    clear()
    print("➕ Add New Server\n")

    # يدخل IP فقط أو IP:PORT
    raw_input = inquirer.text(
        message="Enter server IP (or IP:PORT):"
    ).execute().strip()

    if ":" in raw_input:
        host, port_str = raw_input.split(":", 1)
        try:
            port = int(port_str)
        except ValueError:
            print("\n⚠️ Invalid port number. Skipping.\n")
            return
    else:
        host = raw_input
        port = inquirer.number(
            message="Enter server port:", default=27015
        ).execute()

    servers = load_servers()
    servers.append({"host": host, "port": port})
    save_servers(servers)
    print(f"\n✅ Added server {host}:{port}\n")
    input("Press Enter to return to menu...")

def list_servers():
    clear()
    servers = load_servers()
    if not servers:
        print("⚠️  No servers added yet.\n")
        input("Press Enter to return to menu...")
        return
    print("📋 Saved Servers:\n")
    for i, s in enumerate(servers, start=1):
        print(f"{i}. {s['host']}:{s['port']}")
    print("")
    input("Press Enter to return to menu...")

def query_servers():
    clear()
    servers = load_servers()
    if not servers:
        print("⚠️  No servers found. Add one first!\n")
        input("Press Enter to return to menu...")
        return

    print("=======================================")
    print("        🛰️  Sven Co-op Tracker")
    print("=======================================\n")

    for s in servers:
        host, port = s["host"], s["port"]
        try:
            info = a2s.info((host, port), timeout=3.0)
            players = a2s.players((host, port), timeout=3.0)

            print(f"🟢 Server: {host}:{port}")
            print(f"🏷️  Name : {info.server_name}")
            print(f"🗺️  Map  : {info.map_name}")
            print(f"👥 Players ({len(players)}/{info.max_players})")

            if players:
                for i, p in enumerate(players, start=1):
                    name = p.name or "(Unnamed)"
                    print(f"   {str(i).zfill(2)}. {name}")
            else:
                print("   No players online.")

            print("---------------------------------------\n")

        except socket.timeout:
            print(f"🔴 {host}:{port} - Timeout (no response)\n")
        except Exception as e:
            print(f"🔴 {host}:{port} - Error: {e}\n")

    print("✅ Query completed.\n")
    input("Press Enter to return to menu...")

def main():
    while True:
        clear()
        choice = inquirer.select(
            message="🧭 Choose an option:",
            choices=[
                "Add new server",
                "List saved servers",
                "Start tracking",
                "Exit",
            ],
        ).execute()

        if choice == "Add new server":
            add_server()
        elif choice == "List saved servers":
            list_servers()
        elif choice == "Start tracking":
            query_servers()
        else:
            clear()
            print("👋 Exiting... Bye!\n")
            break

if __name__ == "__main__":
    main()
