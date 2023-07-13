using Networking.Packet;
using Networking.Packet.Clientbound;

namespace Networking; 

public class Client {
    public ClientConnection Connection    { get; }
    public PacketManager    PacketManager { get; }
    
    public Client() {
        PacketManager = new();
        Connection    = new ClientConnection(PacketManager);

        PacketManager.registerHandler((int) CommonPackets.ClientBound.SyncClientID, (_, _, data) => {
            Connection.ClientID = ((SyncIDPacket)data).ClientID;
        });
    }

    public void connectToServer(string ip, int port) {
        Connection.connectToServer(ip, port);
    }

    public void sendServerPacket<T>(T data) where T : struct {
        using var packet = new PacketBuffer();
        PacketManager.getInterpreter<T>().writePacket(packet, data);
        Connection.sendData(packet);
    }
}