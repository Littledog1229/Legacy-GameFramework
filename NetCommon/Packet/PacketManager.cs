using Networking.Packet.Clientbound;
using Networking.Packet.Interpreter;

namespace Networking.Packet; 

public sealed class PacketManager {
    private readonly Dictionary<Type, PacketInterpreter> interpreters_by_type           = new();
    private readonly Dictionary<int,  PacketInterpreter> serverbound_interpreters_by_id = new();
    private readonly Dictionary<int,  PacketInterpreter> clientbound_interpreters_by_id = new();

    private readonly Dictionary<int, Action<uint, int, object>> packet_handlers = new();

    private readonly List<InterpretedPacket> interpreted_packets = new();
    private readonly List<InterpretedPacket> handling_packets    = new();

    public void registerServerboundInterpreter   (PacketInterpreter interpreter, int id, Type type) {
        interpreters_by_type.Add(type, interpreter);
        serverbound_interpreters_by_id.Add(id, interpreter);
    }
    public void registerServerboundInterpreter<T>(PacketInterpreter interpreter, int id) where T : struct {
        interpreters_by_type.Add(typeof(T), interpreter);
        serverbound_interpreters_by_id.Add(id, interpreter);
    }
    
    public void registerClientboundInterpreter   (PacketInterpreter interpreter, int id, Type type) {
        interpreters_by_type.Add(type, interpreter);
        clientbound_interpreters_by_id.Add(id, interpreter);
    }
    public void registerClientboundInterpreter<T>(PacketInterpreter interpreter, int id) where T : struct {
        interpreters_by_type.Add(typeof(T), interpreter);
        clientbound_interpreters_by_id.Add(id, interpreter);
    }

    public void registerHandler(int id, Action<uint, int, object> handler) => packet_handlers.Add(id, handler);
    
    public PacketInterpreter getServerboundInterpreter(int id) {
        try { return serverbound_interpreters_by_id[id]; } 
        catch (Exception) { throw new Exception($"A PacketInterpreter of ID [{id}] has not been registered!"); }
    }
    public PacketInterpreter getClientboundInterpreter(int id) {
        try { return clientbound_interpreters_by_id[id]; } 
        catch (Exception) { throw new Exception($"A PacketInterpreter of ID [{id}] has not been registered!"); }
    }
    public PacketInterpreter getInterpreter(Type type) {
        try { return interpreters_by_type[type]; } 
        catch (Exception) { throw new Exception($"A PacketInterpreter of Type [{type.Name}] has not been registered!"); }
    }
    public PacketInterpreter getInterpreter<T>() where T : struct {
        try { return interpreters_by_type[typeof(T)]; } 
        catch (Exception) { throw new Exception($"A PacketInterpreter of Type [{nameof(T)}] has not been registered!"); }
    }

    public void handlePackets() {
        handling_packets.Clear();

        lock (interpreted_packets) {
            handling_packets.AddRange(interpreted_packets);
            interpreted_packets.Clear();
        }

        foreach (var packet in handling_packets)
            packet_handlers[packet.PacketID](packet.ClientID, packet.PacketID, packet.Data);
    }

    public PacketManager() {
        // Register Packets
        registerClientboundInterpreter<SyncIDPacket>(new SyncIDPacketInterpreter(), (int) CommonPackets.ClientBound.SyncClientID);
    }
    
    public void addServerboundPacket(uint client, PacketBuffer buffer) {
        var packet_id = buffer.readInt();
        if (!serverbound_interpreters_by_id.ContainsKey(packet_id))
            throw new Exception($"Serverbound PacketInterpreter with ID [{packet_id}] does not exist!");
        
        var packet_data = serverbound_interpreters_by_id[packet_id].readPacket(buffer);
        var packet = new InterpretedPacket(client, packet_id, packet_data);

        lock (interpreted_packets)
            interpreted_packets.Add(packet);
    }
    public void addClientboundPacket(uint client, PacketBuffer buffer) {
        var packet_id = buffer.readInt();
        if (!clientbound_interpreters_by_id.ContainsKey(packet_id))
            throw new Exception($"Clientbound PacketInterpreter with ID [{packet_id}] does not exist!");
        
        var packet_data = clientbound_interpreters_by_id[packet_id].readPacket(buffer);
        var packet = new InterpretedPacket(client, packet_id, packet_data);

        lock (interpreted_packets)
            interpreted_packets.Add(packet);
    }

    private readonly struct InterpretedPacket {
        public uint   ClientID { get; init; }
        public int    PacketID { get; init; }
        public object Data     { get; init; }

        public InterpretedPacket(uint client_id, int packet_id, object data) {
            ClientID = client_id;
            PacketID = packet_id;
            Data     = data;
        }
    }
}