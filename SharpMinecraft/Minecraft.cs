using Engine.Application;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpMinecraft.Block;

namespace SharpMinecraft;

public sealed class Minecraft : Application {
    protected override NativeWindowSettings NativeSettings => new() { Vsync = VSyncMode.On };

    private World.World world;

    protected override void initialize() {
        BlockAtlas.init();
        
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.FrontFace(FrontFaceDirection.Cw);

        world = new World.World();
    }

    protected override void destroy() {
        world.destroy();
    }

    protected override void onUpdate() {
        if (!cursor_state && InputManager.mouseRelease(MouseButton.Left))
            toggleMouseState(true);
        
        if (InputManager.keyRelease(Keys.Escape))
            toggleMouseState(false);
        
        world.update();
    }

    protected override void onFocusGain() {
        toggleMouseState(true);
    }

    protected override void onRender() {
        world.render();
    }

    private bool cursor_state;
    
    private void toggleMouseState(bool active) {
        cursor_state = active;
        Engine.Engine.CursorState = active ? CursorState.Grabbed : CursorState.Normal;
    }
}