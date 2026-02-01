# SvenScope 🎮

A lightweight command-line tool for tracking and monitoring Sven Co-op game servers in real-time.

## Features

- **Server Management**: Add, list, and delete servers with ease
- **Real-time Tracking**: Query servers to see live player counts and current maps
- **Player Monitoring**: View all connected players on each server
- **Persistent Storage**: Automatically saves your server list locally
- **Clean Interface**: Simple, professional terminal UI

## Requirements

- .NET 10.0 or higher
- Windows, Linux, or macOS

## Installation

### Download Release
1. Download the latest release from [Releases](../../releases)
2. Extract the ZIP file
3. Run `SvenScope.exe` (Windows) or `./SvenScope` (Linux/macOS)

### Build from Source
```bash
git clone https://github.com/MR11Robot/SvenScope.git
cd SvenScope
dotnet build -c Release
dotnet run
```

## Usage

### Main Menu
```
═══════════════════════════════════════
       SVEN CO-OP SERVER TRACKER
═══════════════════════════════════════

  1. Add new server
  2. List saved servers
  3. Delete server
  4. Start tracking
  5. Exit
```

### Adding a Server
You can add servers in two ways:
- **IP:PORT format**: `192.168.1.100:27015`
- **IP only** (defaults to port 27015): `192.168.1.100`

### Tracking Servers
View real-time information including:
- Server name and IP
- Current map
- Player count
- List of connected players

## Example Output

```
  Server: 192.168.1.100:27015
  Name  : My Sven Co-op Server
  Map   : sc_crossfire
  Players: 5/16

    01. Player1
    02. Player2
    03. Player3
    04. Player4
    05. Player5
```

## Technical Details

- Uses **A2S_INFO** and **A2S_PLAYER** protocols for server queries
- UDP-based communication with 3-second timeout
- JSON-based local storage (`servers.json`)
- Supports challenge-response authentication

## Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features
- Submit pull requests

## License

This project is open source and available under the MIT License.

## Author

Made with ❤️ for the Sven Co-op community

---

**Note**: This tool requires that the game servers have query protocol enabled.
