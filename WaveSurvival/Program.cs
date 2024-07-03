using Engine;

namespace WaveSurvival;

public static class Program {
    public static int Main(string[] args) {
        var app = new WaveSurvival();
        ApplicationCore.ApplicationCore.create(app);
        
        return 0;
    }
}