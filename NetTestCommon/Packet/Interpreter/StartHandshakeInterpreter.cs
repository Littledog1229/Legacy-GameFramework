using Networking.Packet;

namespace NetTestCommon.Packet.Clientbound; 

public sealed class StartHandshakeInterpreter : PacketInterpreter {
    public override object readPacket(PacketBuffer buffer) { return new StartHandshakePacket(); }

    public override void writePacket(PacketBuffer buffer, object data) {
        buffer.write((int) ClientboundPackets.StartHandshake);
    }
}