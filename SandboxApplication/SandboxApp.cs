using ApplicationCore.Application;
using ApplicationCore.Render;
using Engine.Physics;
using Engine.Scenes;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Sandbox; 

public sealed class SandboxApp : Application {
    protected override NativeWindowSettings NativeSettings => new() { Title = "Sandbox App", Vsync = VSyncMode.On };

    protected override void registerEngineSystems() {
        registerEngineSystem<SceneManager>(SceneManager.create());
        registerEngineSystem<PhysicsManager>(PhysicsManager.create());
    }

    protected override void initialize() {
        RenderManager.ClearColor = Color4.Gray;
        
        //SceneManager.addScene(new LegacyEditorScene(), SceneManager.SceneAddMode.Additive);
        SceneManager.addScene(new EditorScene());
    }
}