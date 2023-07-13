namespace SharpMinecraft;

public static class Entrypoint {
    public static int Main(string[] args) {
        var minecraft = new Minecraft();
        Engine.Engine.create(minecraft);

        return 0;
    }
}