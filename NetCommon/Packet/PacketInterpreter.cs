namespace Networking.Packet; 

public abstract class PacketInterpreter {
    public abstract object readPacket  (PacketBuffer buffer);
    public abstract void   writePacket (PacketBuffer buffer, object data);
}