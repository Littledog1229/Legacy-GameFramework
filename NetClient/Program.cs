using NetClient;
using Networking;

public static class Program {
    public static int Main(string[] args) {
        var client = new ProgramClient();
        client.run();

        return 0;
    }
}