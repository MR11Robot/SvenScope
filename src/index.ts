import * as fs from 'fs';
import * as dgram from 'dgram';
import * as readline from 'readline';

interface ServerInfo {
    host: string;
    port: number;
}

interface A2SInfo {
    serverName: string;
    mapName: string;
    players: number;
    maxPlayers: number;
}

interface A2SPlayer {
    name: string;
    duration: number;
}

const SERVERS_FILE = 'servers.json';

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

function question(prompt: string): Promise<string> {
    return new Promise((resolve) => {
        rl.question(prompt, resolve);
    });
}

function clear(): void {
    console.clear();
}

function loadServers(): ServerInfo[] {
    if (!fs.existsSync(SERVERS_FILE)) {
        return [];
    }
    const json = fs.readFileSync(SERVERS_FILE, 'utf8');
    return JSON.parse(json);
}

function saveServers(servers: ServerInfo[]): void {
    const json = JSON.stringify(servers, null, 2);
    fs.writeFileSync(SERVERS_FILE, json);
}

async function addServer(): Promise<void> {
    clear();
    console.log('═══════════════════════════════════════');
    console.log('            ADD NEW SERVER');
    console.log('═══════════════════════════════════════\n');
    
    const rawInput = (await question('  Enter server IP (or IP:PORT): ')).trim();

    if (!rawInput) {
        console.log('\n  [!] Server IP cannot be empty.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    let host: string;
    let port: number;

    if (rawInput.includes(':')) {
        const parts = rawInput.split(':', 2);
        host = parts[0]?.trim() || '';

        if (!host) {
            console.log('\n  [!] Server IP cannot be empty.\n');
            await question('  Press Enter to return to menu...');
            return;
        }

        port = parseInt(parts[1] || '');
        if (isNaN(port) || port < 1 || port > 65535) {
            console.log('\n  [!] Invalid port number. Must be between 1 and 65535.\n');
            await question('  Press Enter to return to menu...');
            return;
        }
    } else {
        host = rawInput;
        const portInput = (await question('  Enter server port (default 27015): ')).trim();

        if (!portInput) {
            port = 27015;
        } else {
            port = parseInt(portInput);
            if (isNaN(port) || port < 1 || port > 65535) {
                console.log('\n  [!] Invalid port number. Must be between 1 and 65535.\n');
                await question('  Press Enter to return to menu...');
                return;
            }
        }
    }

    const servers = loadServers();

    if (servers.some(s => s.host === host && s.port === port)) {
        console.log(`\n  [!] Server ${host}:${port} already exists.\n`);
        await question('  Press Enter to return to menu...');
        return;
    }

    servers.push({ host, port });
    saveServers(servers);

    console.log(`\n  [+] Server added: ${host}:${port}\n`);
    await question('  Press Enter to return to menu...');
}

async function listServers(): Promise<void> {
    clear();
    const servers = loadServers();

    console.log('═══════════════════════════════════════');
    console.log('            SAVED SERVERS');
    console.log('═══════════════════════════════════════\n');

    if (servers.length === 0) {
        console.log('  [!] No servers added yet.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    servers.forEach((server, i) => {
        console.log(`  ${i + 1}. ${server.host}:${server.port}`);
    });

    console.log();
    await question('  Press Enter to return to menu...');
}

async function deleteServer(): Promise<void> {
    clear();
    const servers = loadServers();

    console.log('═══════════════════════════════════════');
    console.log('            DELETE SERVER');
    console.log('═══════════════════════════════════════\n');

    if (servers.length === 0) {
        console.log('  [!] No servers added yet.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    console.log('  Saved servers:\n');
    servers.forEach((server, i) => {
        console.log(`  ${i + 1}. ${server.host}:${server.port}`);
    });

    const input = (await question('\n  Enter server number to delete (0 to cancel): ')).trim();

    if (!input) {
        console.log('\n  [!] Invalid input.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    const choice = parseInt(input);
    if (isNaN(choice) || choice < 0 || choice > servers.length) {
        console.log('\n  [!] Invalid server number.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    if (choice === 0) {
        console.log('\n  [!] Deletion cancelled.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    const serverToDelete = servers[choice - 1];
    if (!serverToDelete) {
        console.log('\n  [!] Server not found.\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    servers.splice(choice - 1, 1);
    saveServers(servers);

    console.log(`\n  [+] Server deleted: ${serverToDelete.host}:${serverToDelete.port}\n`);
    await question('  Press Enter to return to menu...');
}

function queryServerInfo(host: string, port: number): Promise<A2SInfo> {
    return new Promise((resolve, reject) => {
        const client = dgram.createSocket('udp4');
        const timeout = setTimeout(() => {
            client.close();
            reject(new Error('Timeout'));
        }, 3000);

        const request = Buffer.from([
            0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75,
            0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69,
            0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00
        ]);

        client.on('message', (msg) => {
            clearTimeout(timeout);
            client.close();
            resolve(parseA2SInfo(msg));
        });

        client.on('error', (err) => {
            clearTimeout(timeout);
            client.close();
            reject(err);
        });

        client.send(request, port, host);
    });
}

function parseA2SInfo(data: Buffer): A2SInfo {
    let index = 5;
    const serverName = readString(data, index);
    index += serverName.length + 1;
    const mapName = readString(data, index);
    index += mapName.length + 1;
    const folder = readString(data, index);
    index += folder.length + 1;
    const game = readString(data, index);
    index += game.length + 1;
    index += 2;
    const players = data[index++] || 0;
    const maxPlayers = data[index++] || 0;

    return {
        serverName,
        mapName,
        players,
        maxPlayers
    };
}

function queryServerPlayers(host: string, port: number): Promise<A2SPlayer[]> {
    return new Promise((resolve, reject) => {
        const client = dgram.createSocket('udp4');
        const timeout = setTimeout(() => {
            client.close();
            reject(new Error('Timeout'));
        }, 3000);

        const request = Buffer.from([0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF]);

        let step = 0;

        client.on('message', (msg) => {
            if (step === 0 && msg[4] === 0x41) {
                step = 1;
                const challenge = msg.slice(5, 9);
                const requestWithChallenge = Buffer.from([
                    0xFF, 0xFF, 0xFF, 0xFF, 0x55,
                    challenge[0] || 0, challenge[1] || 0, challenge[2] || 0, challenge[3] || 0
                ]);
                client.send(requestWithChallenge, port, host);
            } else {
                clearTimeout(timeout);
                client.close();
                resolve(parseA2SPlayers(msg));
            }
        });

        client.on('error', (err) => {
            clearTimeout(timeout);
            client.close();
            reject(err);
        });

        client.send(request, port, host);
    });
}

function parseA2SPlayers(data: Buffer): A2SPlayer[] {
    const players: A2SPlayer[] = [];
    let index = 5;
    const count = data[index++] || 0;

    for (let i = 0; i < count; i++) {
        index++;
        const name = readString(data, index);
        index += name.length + 1;
        index += 4;
        const duration = data.readFloatLE ? data.readFloatLE(index) : 0;
        index += 4;

        players.push({ name, duration });
    }

    return players;
}

function readString(data: Buffer, index: number): string {
    let str = '';
    while (index < data.length && data[index] !== 0) {
        str += String.fromCharCode(data[index] || 0);
        index++;
    }
    return str;
}

async function queryServers(): Promise<void> {
    clear();
    const servers = loadServers();

    if (servers.length === 0) {
        console.log('═══════════════════════════════════════');
        console.log('          SERVER TRACKING');
        console.log('═══════════════════════════════════════\n');
        console.log('  [!] No servers found. Add one first!\n');
        await question('  Press Enter to return to menu...');
        return;
    }

    console.log('═══════════════════════════════════════');
    console.log('          SERVER TRACKING');
    console.log('═══════════════════════════════════════\n');

    for (const server of servers) {
        try {
            const info = await queryServerInfo(server.host, server.port);
            const players = await queryServerPlayers(server.host, server.port);

            console.log(`  Server: ${server.host}:${server.port}`);
            console.log(`  Name  : ${info.serverName}`);
            console.log(`  Map   : ${info.mapName}`);
            console.log(`  Players: ${players.length}/${info.maxPlayers}`);

            if (players.length > 0) {
                console.log();
                players.forEach((player, i) => {
                    const name = player.name || '(Unnamed)';
                    console.log(`    ${(i + 1).toString().padStart(2, '0')}. ${name}`);
                });
            } else {
                console.log('    No players online.');
            }
            console.log('\n───────────────────────────────────────\n');
        } catch (err) {
            console.log(`  [X] ${server.host}:${server.port}`);
            if (err instanceof Error && err.message === 'Timeout') {
                console.log('      Timeout - Server not responding\n');
            } else {
                console.log(`      Error: ${err instanceof Error ? err.message : 'Unknown error'}\n`);
            }
            console.log('───────────────────────────────────────\n');
        }
    }

    console.log('  [+] Query completed.\n');
    await question('  Press Enter to return to menu...');
}

async function main(): Promise<void> {
    while (true) {
        clear();
        console.log('═══════════════════════════════════════');
        console.log('       SVEN CO-OP SERVER TRACKER');
        console.log('═══════════════════════════════════════\n');
        console.log('  1. Add new server');
        console.log('  2. List saved servers');
        console.log('  3. Delete server');
        console.log('  4. Start tracking');
        console.log('  5. Exit\n');

        const choice = await question('  Your choice: ');

        switch (choice) {
            case '1':
                await addServer();
                break;
            case '2':
                await listServers();
                break;
            case '3':
                await deleteServer();
                break;
            case '4':
                await queryServers();
                break;
            case '5':
                clear();
                console.log('\n  Exiting... Goodbye!\n');
                rl.close();
                return;
            default:
                console.log('\n  [!] Invalid choice. Try again.');
                await new Promise(resolve => setTimeout(resolve, 1500));
                break;
        }
    }
}

main();