using Networking.Packet.Clientbound;

namespace Networking.Packet.Interpreter; 

public sealed class SyncIDPacketInterpreter : PacketInterpreter {
    public override object readPacket(PacketBuffer buffer) {
        var sync_packet = new SyncIDPacket {
            ClientID = buffer.readUInt()
        };

        return sync_packet;
    }

    public override void writePacket(PacketBuffer buffer, object data) {
        var sync_packet = (SyncIDPacket) data;
        buffer.write((int) CommonPackets.ClientBound.SyncClientID);
        buffer.write(sync_packet.ClientID);
    }
}