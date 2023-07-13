using NetTestCommon;
using NetTestCommon.Packet;
using NetTestCommon.Packet.Clientbound;
using Networking;

namespace Sandbox; 

public sealed class TestServer : Server {
    private Dictionary<uint, ConnectedClient> clients = new();

    protected override void onInit() {
        PacketManager.registerServerboundInterpreter<EndHandshakePacket>(new EndHandshakeInterpreter(), (int) ServerboundPackets.FinishHandshake);
        PacketManager.registerClientboundInterpreter<StartHandshakePacket>(new StartHandshakeInterpreter(), (int) ClientboundPackets.StartHandshake);
        
        PacketManager.registerHandler((int) ServerboundPackets.FinishHandshake, finishHandshake);
    }

    private void finishHandshake(uint client_id, int packet_id, object data) {
        if (!clients.ContainsKey(client_id))
            return;
        
        var packet = (EndHandshakePacket) data;
        clients[client_id].State = PlayState.Play;
        clients[client_id].Username = packet.Username;
        
        Console.WriteLine($" . Client [{clients[client_id].Connection.Socket.Client.RemoteEndPoint}] has joined with username '{clients[client_id].Username}'!");
    }

    protected override void onClientConnect(ClientConnection connection) {
        Console.WriteLine($"Client connected to server: {connection.Socket.Client.RemoteEndPoint}");
        sendClientPacket(connection.ClientID, new StartHandshakePacket());
        
        clients.Add(connection.ClientID, new ConnectedClient(connection, "", PlayState.Handshake));
    }

    protected override void onClientDisconnect(ClientConnection connection) {
        Console.WriteLine($"Client disconnected: {connection.Socket.Client.RemoteEndPoint}");
        clients.Remove(connection.ClientID);
    }
}