using Networking;
using Sandbox;

public static class Program {
    public static int Main(string[] args) {
        Console.WriteLine("Starting Server");
        var server = new TestServer();
        server.start(7777);

        while (true) {
            server.handlePackets();
        }
    }
}