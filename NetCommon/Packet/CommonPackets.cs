namespace Networking.Packet; 

public static class CommonPackets {
    public enum ServerBound {
        Last = -1
    }

    public enum ClientBound {
        SyncClientID = 0,
        Last = 0
    }
}