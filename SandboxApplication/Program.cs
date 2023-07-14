namespace Sandbox;

public static class Program {
    public static int Main(string[] args) {
        var app = new SandboxApp();
        ApplicationCore.ApplicationCore.create(app);
        
        return 0;
    } 
}