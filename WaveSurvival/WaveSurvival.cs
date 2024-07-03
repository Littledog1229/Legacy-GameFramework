using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.UI;
using Engine.Scenes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace WaveSurvival; 

public sealed class WaveSurvival : Application {
    protected override NativeWindowSettings NativeSettings => new() {
        Vsync = VSyncMode.On
    };

    protected override void registerEngineSystems() {
        registerEngineSystem<UIManager>(UIManager.create());
        registerEngineSystem<SceneManager>(SceneManager.create());
    }

    protected override void initialize() {
        SceneManager.addScene(new SingleplayerScene());
        
        GL.Enable(EnableCap.DepthTest);
        RenderManager.ClearColor = Color4.Gray;
    }
}