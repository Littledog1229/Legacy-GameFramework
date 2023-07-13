using Networking.Packet;

namespace NetTestCommon.Packet;

public enum ServerboundPackets {
    FinishHandshake = CommonPackets.ServerBound.Last + 1,
    SendMessage
}

public enum ClientboundPackets {
    StartHandshake = CommonPackets.ClientBound.Last + 1,
    ReceiveMessage
}