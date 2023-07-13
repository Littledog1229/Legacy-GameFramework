using System.Net.Sockets;
using Networking.Packet;

namespace Networking; 

public sealed class ClientConnection : IDisposable {
    public const int DATA_BUFFER_SIZE = 4096;
    
    public Server?        Owner         { get; private  set; } // Only set on the servers side
    public PacketManager  PacketManager { get; private  set; }
    public TcpClient      Socket        { get; private  set; }
    public ConnectionType Type          { get; private  set; }
    public uint           ClientID      { get; internal set; }
    public bool           Valid         { get; private  set; }
    public bool           Connecting    { get; private set; }

    private          NetworkStream stream         = null!;
    private readonly PacketBuffer  next_packet    = null!;
    private readonly byte[]        receive_buffer = null!;

    public ClientConnection(PacketManager manager) {
        Owner         = null;
        PacketManager = manager;
        Socket        = null!;
        Type          = ConnectionType.Server;
        ClientID      = 0;
        Valid         = true;

        next_packet    = new PacketBuffer();
        receive_buffer = new byte[DATA_BUFFER_SIZE];
    }
    
    public ClientConnection(Server owner, PacketManager manager, TcpClient socket, uint client_id) {
        Owner         = owner;
        PacketManager = manager;
        Socket        = socket;
        Type          = ConnectionType.Client;
        ClientID      = client_id;
        Valid         = true;

        socket.SendBufferSize    = DATA_BUFFER_SIZE;
        socket.ReceiveBufferSize = DATA_BUFFER_SIZE;

        stream         = socket.GetStream();
        next_packet    = new PacketBuffer();
        receive_buffer = new byte[DATA_BUFFER_SIZE];
        
        startRead();
    }
    ~ClientConnection() => Dispose();

    // Method used to connect to a server from a client
    public void connectToServer(string ip, int port) {
        Socket = new TcpClient {
            SendBufferSize = DATA_BUFFER_SIZE,
            ReceiveBufferSize = DATA_BUFFER_SIZE
        };

        Connecting = true;
        Socket.BeginConnect(ip, port, connectCallback, null);
    }

    private void startRead() => stream.BeginRead(receive_buffer, 0, DATA_BUFFER_SIZE, receiveData, null);

    private void connectCallback(IAsyncResult result) {
        try {
            Socket.EndConnect(result);

            if (!Socket.Connected)
                return;
        
            stream = Socket.GetStream();
            Connecting = false;
            startRead();
        } catch (Exception e) {
            Console.WriteLine($"Unable to connect: {e.Message}");
            Connecting = false;
        }
    }
    
    public void sendData(PacketBuffer data_buffer) {
        if (!Valid)
            return;
        
        try {
            data_buffer.finalize();
            
            if (Socket != null!)
                stream.BeginWrite(data_buffer.read_buffer!, 0, data_buffer.Length, null, null);
        }
        catch (Exception e) {
            Console.WriteLine($"Error sending data to socket: {e}");
            disconnect();
        }
    }

    public void disconnect() {
        if (!Valid)
            return;
        
        Owner?.disconnectClient(this);

        Socket.Close();
        stream = null!;

        Valid = false;
    }
    
    private void receiveData(IAsyncResult result) {
        try {
            var bytes = stream.EndRead(result);

            if (bytes <= 0) {
                disconnect();
                return;
            }

            var data = new byte[bytes];
            Array.Copy(receive_buffer, data, bytes);
            
            if (interpretData(data))
                next_packet.reset();

            stream.BeginRead(receive_buffer, 0, DATA_BUFFER_SIZE, receiveData, null);
        } catch (Exception e) {
            Console.WriteLine($"Failed to read data: {e.Message}");
            disconnect();
        }
    }

    private bool interpretData(IEnumerable<byte> data) {
        var packet_length = 0;
        
        next_packet.setBytes(data);
        if (next_packet.RemainingLength >= 4) {
            packet_length = next_packet.readInt();
            if (packet_length <= 0)
                return true; // The packet does not contain any data besides the size, making it a malformed packet
        }

        while (packet_length > 0 && packet_length <= next_packet.RemainingLength) {
            var bytes = next_packet.readBytes(packet_length);
            using var packet = new PacketBuffer(bytes);
            
            if (Type == ConnectionType.Client)
                PacketManager.addServerboundPacket(ClientID, packet);
            else
                PacketManager.addClientboundPacket(ClientID, packet);

            packet_length = 0;
            if (next_packet.RemainingLength < 4) 
                continue;
            
            packet_length = next_packet.readInt();
            if (packet_length <= 0)
                return true;
        }
        
        return packet_length <= 1;
    }

    public bool Disposed { get; private set; }
    
    public void Dispose() {
        if (Disposed)
            return;
        
        next_packet.Dispose();
        stream.Dispose();
        Socket.Dispose();

        Disposed = true;
        GC.SuppressFinalize(this);
    }

    public enum ConnectionType {
        Client,
        Server
    }
}