using System.Runtime;
using Networking.Packet;

namespace NetTestCommon.Packet.Clientbound; 

public sealed class EndHandshakeInterpreter : PacketInterpreter {
    public override object readPacket(PacketBuffer buffer) {
        return new EndHandshakePacket {
            Username = buffer.readString()
        };
    }

    public override void writePacket(PacketBuffer buffer, object data) {
        var packet = (EndHandshakePacket) data;
        
        buffer.write((int) ServerboundPackets.FinishHandshake);
        buffer.write(packet.Username);
    }
}