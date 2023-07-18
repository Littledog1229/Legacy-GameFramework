using System.Reflection;
using ApplicationCore.Render;
using Engine;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ApplicationCore.Application; 

public sealed class ApplicationManager : GameWindow {
    public static ApplicationManager Instance { get; private set; } = null!;
    
    public Application Application { get; private set; }

    private ApplicationManager(Application application) : base(GameWindowSettings.Default, application.NativeSettings) {
        Application = application;
    }

    private double last_time = 0.0d;
    private float  fixed_update_interval   = 0.02f;
    private float  time_since_fixed_update = 0.0f;
    
    public static int WindowWidth  { get; private set; }
    public static int WindowHeight { get; private set; }
    
    protected override void OnLoad() {
        base.OnLoad();
        
        // Currently a "constant" (not specified in code as this can change)
        EngineCore.setFixedDeltaTime(fixed_update_interval);
        
        // Initialize all engine systems
        RenderManager.initialize();
        EngineCore.initialize();
        Application.initialize();
        
        // Start all engine systems
        RenderManager.start();
        EngineCore.start();
        Application.start();
    }

    protected override void OnUnload() {
        base.OnUnload();
        
        // Unload all engine systems
        Application.unload();
        EngineCore.unload();
        RenderManager.unload();
        
        // Destroy all engine systems (to free resources)
        Application.destroy();
        EngineCore.destroy();
        RenderManager.destroy();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        var current_time = GLFW.GetTime();
        var dt = current_time - last_time;
        last_time = current_time;
        
        EngineCore.setPreciseDeltaTime(dt);

        // Fixed Update
        time_since_fixed_update += (float) dt;
        while (time_since_fixed_update >= fixed_update_interval) {
            time_since_fixed_update -= fixed_update_interval;
            
            // FixedUpdate for all engine systems
            EngineCore.fixedUpdate();
            Application.fixedUpdate();
        }
        
        // Update all engine systems
        EngineCore.update();
        Application.update();
        
        // LateUpdate for all engine systems
        EngineCore.lateUpdate();
        Application.lateUpdate();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);
        
        // PreRender all engine systems
        RenderManager.preRender();
        EngineCore.preRender();
        Application.preRender();
        
        // Render all engine systems
        RenderManager.render();
        EngineCore.render();
        Application.render();
        
        // PostRender all engine systems
        Application.postRender();
        EngineCore.postRender();
        RenderManager.postRender();
        
        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        
        Application.onResize(e.Width, e.Height);
        RenderManager.resize(e.Width, e.Height);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e) {
        base.OnFocusedChanged(e);
        
        Application.onFocusChange(e.IsFocused);
    }

    internal static void create(Application application) {
        if (Instance != null!)
            throw new ApplicationException("An application has already been created!");

        Instance = new(application);
        RenderManager.resize(application.NativeSettings.Size.X, application.NativeSettings.Size.Y);

        EngineCore.setEngineResourceAssembly (Assembly.GetAssembly(typeof(ApplicationManager))!);
        EngineCore.setApplicationAssembly    (Assembly.GetAssembly(application.GetType())!);
        
        // Register required engine systems
        EngineCore.registerEngineSystem(InputManager.create());
        
        // Allow the application to register engine systems
        application.registerEngineSystems();

        Instance.last_time = GLFW.GetTime();
        Instance.Run();
    }
}