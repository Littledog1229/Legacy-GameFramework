using System.Reflection;
using Engine.Render;
using Engine.UI;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;

namespace Engine.Application; 

// TODO: Swap to inheriting native window or raw GLFW
internal sealed class WindowManager : GameWindow {
    private static WindowManager Instance { get; set; } = null!;

    public static MouseCursor MouseCursor      => Instance.Cursor;

    public static CursorState MouseCursorState {
        get => Instance.CursorState;
        set => Instance.CursorState = value;
    }

    private Application application = null!;
    
    internal static void create(Application application) {
        if (Instance != null!)
            return;

        Instance             = new WindowManager(application.GameSettings, application.NativeSettings);
        Instance.application = application;

        Engine.ApplicationAssembly = Assembly.GetAssembly(application.GetType())!;
        
        Instance.Run();
    }

    internal static void swapBuffers() => Instance.SwapBuffers();

    private WindowManager(GameWindowSettings game, NativeWindowSettings native) : base(game, native) { }

    protected override void OnLoad() {
        RenderManager.initialize();

#if DEBUG
        debugInit();
#endif
        
        InputManager.create();

        KeyDown    += InputManager.Instance.keyDown;
        KeyUp      += InputManager.Instance.keyUp;
        MouseDown  += InputManager.Instance.mouseDown;
        MouseUp    += InputManager.Instance.mouseUp;
        MouseWheel += InputManager.Instance.mouseWheel;
        MouseMove  += InputManager.Instance.mouseMove;
        
        //UIManager.create();
        
        application.initialize();
        application.start();
    }

    protected override void OnUnload() {
        base.OnUnload();
        
        application.destroy();
        RenderManager.destroy();
        
        Dispose();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        Engine.DeltaTime = (float) args.Time;
        Engine.PreciseDeltaTime = args.Time;
        
        //UIManager.update();

        application.onUpdate();
        // TODO: Fixed Update
        
        InputManager.lateUpdate();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        
        RenderManager.startFrame();
        application.onRender();
        //UIManager.render();
        RenderManager.endFrame();
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        RenderManager.resize(e.Width, e.Height);
        application.onResize();
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e) {
        base.OnFocusedChanged(e);
        
        if (e.IsFocused)
            application.onFocusGain();
        else 
            application.onFocusLost();
    }

#if DEBUG
    private void debugInit() {
        Console.WriteLine(" . Debug Initialization");
    }
#endif
}