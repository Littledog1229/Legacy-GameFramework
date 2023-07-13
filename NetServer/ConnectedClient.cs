using NetTestCommon;
using Networking;

namespace Sandbox;

public class ConnectedClient {
    public ClientConnection Connection { get; private set; }
    public string           Username   { get; set; }
    public PlayState        State      { get; set; }

    public ConnectedClient(ClientConnection connection, string username, PlayState state) {
        Connection = connection;
        Username   = username;
        State      = state;
    }
}