using ApplicationCore.Application;
using Engine.Scenes;

namespace Sandbox3D;

public sealed class Sandbox : Application {
    protected override void registerEngineSystems() {
        registerEngineSystem<SceneManager>(SceneManager.create());
    }

    protected override void initialize() {
        SceneManager.addScene(new GeneratorScene());
    }
}