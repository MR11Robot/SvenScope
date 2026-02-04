# SvenScope 🎮

A command-line tool for tracking Sven Co-op game servers using the A2S protocol.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue)
![Node](https://img.shields.io/badge/node-%3E%3D18-green)

## Features ✨

- 🔍 Query Sven Co-op servers in real-time
- 👥 View active players and server info
- 💾 Save and manage multiple servers
- 🚀 Fast and lightweight
- 🖥️ Cross-platform support (Windows, macOS, Linux)

## Installation 📦

### From Release (Recommended)

Download the latest release for your platform:

- **Windows**: `svenscope-win.exe`
- **macOS**: `svenscope-macos`
- **Linux**: `svenscope-linux`

[Download Latest Release](https://github.com/MR11Robot/SvenScope/releases)

### From Source
```bash
# Clone the repository
git clone https://github.com/MR11Robot/SvenScope.git
cd SvenScope

# Install dependencies
npm install

# Run the application
npm start
```

## Usage 🎯

Run the executable or use `npm start`:
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
```
IP:PORT format: 192.168.1.100:27015
```

or
```
IP: 192.168.1.100
Port: 27015 (default)
```

### Tracking Servers

Select option 4 to query all saved servers and view:
- Server name
- Current map
- Player count
- Active player list

## Building from Source 🔨
```bash
# Install dependencies
npm install -D @types/node

# Build TypeScript
npm run build

# Create executables for all platforms
npm run package
```

## Development 💻
```bash
# Run in development mode
npm run dev

# Type check
npm run type-check

# Build
npm run build
```

## Technologies Used 🛠️

- **TypeScript** - Type-safe JavaScript
- **Node.js** - Runtime environment
- **UDP Sockets** - A2S protocol communication
- **pkg** - Executable packaging

## A2S Protocol 📡

This tool implements the Source engine's A2S protocol for querying game servers:
- `A2S_INFO` - Server information
- `A2S_PLAYER` - Player list with connection time


## License 📄

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author 👨‍💻

**MR11Robot**

- GitHub: [@MR11Robot](https://github.com/MR11Robot)

## Support ⭐

If you find this tool useful, please give it a star on GitHub!

## Acknowledgments 🙏

- Sven Co-op community
- Valve's Source engine A2S protocol documentation
