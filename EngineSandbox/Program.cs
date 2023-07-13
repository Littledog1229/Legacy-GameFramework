using Engine.Application;

namespace Sandbox; 

public static class Program {
    // ReSharper disable once InconsistentNaming
    public static int Main(string[] args) {
        var app = new SandboxApp();
        Engine.Engine.create(app);
        
        return 0;
    }
}