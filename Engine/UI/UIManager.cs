using System.Diagnostics;
using Engine.Application;
using Engine.Render;
using Engine.Render.Batch;
using Engine.Render.Camera;
using Engine.UI.Element;
using OpenTK.Mathematics;

namespace Engine.UI; 

public sealed class UIManager {
    private static UIManager Instance { get; set; } = null!;

    private readonly Panel root_panel = new(new Vector2(0, 0), new Vector2(1920, 1080));
    private UIObject?      focused;

    private UIFlags flags;

    public static UIFlags Flags {
        get => Instance.flags;
        set => Instance.flags = value;
    } 

    private UICamera camera;
    private UIBatch  batch;

    internal static void resize() => Instance.internalResize();
    
    internal static void create() {
        if (Instance != null!)
            return;

        Instance = new UIManager();
        Instance.internalInit();
    }

    internal static void update() => Instance.internalUpdate();
    internal static void render() => Instance.internalRender();
    
    private void internalInit() {
        var panel       = new Panel(new Vector2(10, 10), new Vector2(600, 200)) {
            Focusable = true
        };

        var element = new BoxElement(new Vector2(15, 15), new Vector2(100, 100));
        panel.addElement("box", element);
        element.Focusable = true;

        var button = new TestButtonElement(new Vector2(130, 15), new Vector2(200, 100));
        button.Focusable = true;
        button.OnPress += (button_element) => {
            button_element.NormalColor = new Color4(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1.0f);
            Console.WriteLine("PRESSED");
        };
        
        panel.addElement("button", button);
        
        root_panel.addChild("test", panel);

        camera = new UICamera();
        batch  = new UIBatch();
        RenderManager.addCamera(camera);
        
        camera.initialize();
        batch.initialize();
    }

    private void internalResize() {
        root_panel.resize(RenderManager.WindowWidth, RenderManager.WindowHeight);
    }

    private void internalUpdate() {
        if (InputManager.MouseMoved)
            flags.addFlag(UIFlags.FocusDirty);
        
        if (!flags.hasFlag(UIFlags.FocusLocked) && flags.hasFlag(UIFlags.FocusDirty))
            refocus();
        
        root_panel.update();
    }

    private void internalRender() {
        batch.begin(camera);
        root_panel.render(batch);
        batch.end();
    }

    private void refocus() {
        var mouse_position = camera.screenToUISpace(InputManager.MousePosition);
        
        var new_focused = root_panel.findFocus(mouse_position);
        if (new_focused == focused)
            return;

        if (new_focused != null)
            new_focused.Focused = true;
        if (focused != null)
            focused.Focused = false;
        
        new_focused?.focusGained();
        focused?.focusLost();

        focused = new_focused;
        Console.WriteLine($"Focused Changed to: {focused}");
    }
}