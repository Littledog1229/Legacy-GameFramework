

using ApplicationCore.Application;
using Engine;

namespace Sandbox3D;

public static class Program {
    public static int Main(string[] args) {
        var app = new Sandbox();
        ApplicationCore.ApplicationCore.create(app);
        
        return 0;
    }
}