using System.Diagnostics;
using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.Render.Camera;
using ApplicationCore.UI.Elements;
using Engine;
using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace ApplicationCore.UI; 

// General Info:
//  . The user is responsible for resizing, its as simple as add the resize resize() method to RenderManager.OnResize
//  . The size of the root determines the size of the entire UI Region, and is not restricted to the window size
//     : This is so that you can render UI in a viewport, such as a game view in a scene editor.

// TODO: Optimize Focusability, right now focus is marked dirty when any elements focusability changes
// TODO: Make UI based on the top-right of the screen.

public sealed class UIManager : EngineSystem {
    private static UIManager instance { get; set; } = null!;
    
    public static UIObject  Root          { get; } = new RootPanel();
    public static UIObject? FocusedObject { get; private set; }
    public static UIFlags   Flags         { get; set; } = UIFlags.None;

    public static Func<Vector2> MousePositionProvider { get; set; } = () => InputManager.MousePosition;

    private UICamera camera = null!;
    private UIBatch  batch  = null!;

    public static void render(RenderPipeline pipeline) => instance.internalRender(pipeline);
    
    public static UIManager create() {
        if (instance != null!)
            return instance;

        instance              = new UIManager();
        Root.CanFocusChildren = true;
        Root.Focusable        = false;
        
        return instance;
    }
    
    public static void resize(int width, int height) {
        instance.camera.resize(width, height);
        Root.resize(width, height);
    }

    protected override void initialize() {
        camera = new UICamera();
        batch  = new UIBatch();
        
        camera.initialize();
        batch.initialize();
    }

    protected override void update() {
        if (InputManager.MouseMoved)
            Flags |= UIFlags.FocusDirty;

        if ((Flags & UIFlags.FocusDirty) != UIFlags.None && (Flags & UIFlags.FocusLocked) == UIFlags.None)
            refocus();
        
        Root.update();
    }

    private void internalRender(RenderPipeline pipeline) {
        batch.begin(camera);
        Root.render(batch);
        batch.end();
    }

    private void refocus() {
        var mouse_position = MousePositionProvider();

        var new_focused = Root.findFocus(mouse_position);
        if (new_focused == FocusedObject)
            return;

        if (new_focused != null)
            new_focused.Focused = true;
        if (FocusedObject != null)
            FocusedObject.Focused = false;
        
        FocusedObject?.onFocusLost();
        FocusedObject = new_focused;
        FocusedObject?.onFocusGained();
        
        Console.WriteLine($"Focused Changed to: {FocusedObject}");
    }
    
    private UIManager() { }
}