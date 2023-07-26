using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.UI;
using Engine.Physics;
using Engine.Scenes;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Sandbox; 

// TODO: Migrate Scene Editor stuff out
public sealed class SandboxApp : Application {
    protected override NativeWindowSettings NativeSettings => new() { Title = "Sandbox App", Vsync = VSyncMode.On };

    protected override void registerEngineSystems() {
        registerEngineSystem<SceneManager>(SceneManager.create());
        registerEngineSystem<PhysicsManager>(PhysicsManager.create());
        registerEngineSystem<UIManager>(UIManager.create());
    }

    protected override void initialize() {
        RenderManager.ClearColor = Color4.Gray;
        
        //SceneManager.addScene(new LegacyEditorScene(), SceneManager.SceneAddMode.Additive);
        //SceneManager.addScene(new EditorScene());
        //SceneManager.addScene(new UITestScene());
        //SceneManager.addScene(new FontTestingScene());
        SceneManager.addScene(new EcsScene());
    }
}