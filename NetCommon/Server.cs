using System.Net;
using System.Net.Sockets;
using Networking.Packet;

namespace Networking; 

public abstract class Server {
    public int                                Port          { get; private set; }
    public Dictionary<uint, ClientConnection> Connections   { get; } = new();
    protected PacketManager                   PacketManager { get; } = new();

    private TcpListener server           = null!;
    private List<uint>  free_identifiers = new();
    private uint        next_identifier  = 0;
    
    public void start(int port) {
        Port = port;

        onInit();
        
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        server.BeginAcceptTcpClient(clientConnect, null);
        
        onStart();
    }

    public void close() {
        onClose();
        server.Stop();
    }

    public void handlePackets() => PacketManager.handlePackets();
    
    protected virtual void onInit()  { } // Fired before the TcpListener (server) is created and started
    protected virtual void onStart() { } // Fired after  the TcpListener (server) is started
    protected virtual void onClose() { } // Fired before the TcpListener (server) is closed
    
    protected virtual bool acceptConnection(TcpClient? client) { return true; } // Use this method to block a connection under certain conditions, such as if the server is full
    
    protected virtual void onClientConnect    (ClientConnection connection) { } // Method used to handle more advanced client handshaking (at a point where you can send and receive data)
    protected virtual void onClientDisconnect (ClientConnection connection) { }

    public void disconnectClient(ClientConnection connection) {
        onClientDisconnect(connection);

        Connections.Remove(connection.ClientID);
        lock (free_identifiers)
            free_identifiers.Add(connection.ClientID);
    }

    public void sendClientPacket<T>(uint client_id, T data) where T : struct {
        var client = Connections[client_id];
        using var packet = new PacketBuffer();
        PacketManager.getInterpreter<T>().writePacket(packet, data);
        client.sendData(packet);
    }
    
    public void sendClientsPacket<T>(T data) where T : struct {
        using var packet = new PacketBuffer();
        PacketManager.getInterpreter<T>().writePacket(packet, data);

        lock (Connections)
            foreach (var (_, client) in Connections)
                client.sendData(packet);
    }
    
    public void sendClientsPacket<T>(uint excluding_client, T data) where T : struct {
        using var packet = new PacketBuffer();
        PacketManager.getInterpreter<T>().writePacket(packet, data);

        lock (Connections)
            foreach (var (id, client) in Connections)
                if (id != excluding_client)
                    client.sendData(packet);
    }
    
    private void clientConnect(IAsyncResult result) {
        var client_socket = server.EndAcceptTcpClient(result);
        server.BeginAcceptTcpClient(clientConnect, null);

        if (!acceptConnection(client_socket)) {
            client_socket.Close();
            return;
        }

        var client_id = getFreeID();
        var client        = new ClientConnection(this, PacketManager, client_socket, client_id);
        
        Connections.Add(client_id, client);
        onClientConnect(client);
    }

    private uint getFreeID() {
        lock (free_identifiers) {
            if (free_identifiers.Count <= 0)
                return next_identifier++;
        
            var identifier = free_identifiers[0];
            free_identifiers.RemoveAt(0);
            return identifier;
        }
    }
}