using NetTestCommon;
using NetTestCommon.Packet;
using NetTestCommon.Packet.Clientbound;
using Networking;

namespace NetClient; 

public class ProgramClient {
    private readonly Client client = new();
    private PlayState       state = PlayState.Handshake;

    private string username = "";

    public void run() {
        var manager = client.PacketManager;
        manager.registerClientboundInterpreter<StartHandshakePacket> (new StartHandshakeInterpreter(), (int) ClientboundPackets.StartHandshake);
        manager.registerServerboundInterpreter<EndHandshakePacket>   (new EndHandshakeInterpreter(),   (int) ServerboundPackets.FinishHandshake);
        
        manager.registerHandler((int) ClientboundPackets.StartHandshake, startHandshake);
        
        Console.Write("Enter Username:  ");
        username = Console.ReadLine()!;
        Console.Write("Enter Server IP: ");
        var ip = Console.ReadLine()!;
        
        Console.WriteLine("Connecting to server...");
        client.connectToServer(ip, 7777);
        
        while (client.Connection.Connecting) {
            Thread.Sleep(100);
        }

        if (!client.Connection.Socket.Connected)
            return;

        Console.WriteLine("Successfully connected to server!");

        while (state == PlayState.Handshake) { client.Connection.PacketManager.handlePackets(); }
        
        while (true) {
            // TODO: Fancy stuff
            client.Connection.PacketManager.handlePackets();
        }
    }

    private void startHandshake(uint client_id, int packet_id, object _) {
        var packet = new EndHandshakePacket { Username = username };
        client.sendServerPacket(packet);
    }
}