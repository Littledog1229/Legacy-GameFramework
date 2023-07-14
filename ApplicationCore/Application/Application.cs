using ApplicationCore.Render;
using Engine;
using OpenTK.Windowing.Desktop;

namespace ApplicationCore.Application; 

public abstract class Application {
    public static float DeltaTime    => Time.DeltaTime;
    public static int   WindowWidth  => RenderManager.WindowWidth;
    public static int   WindowHeight => RenderManager.WindowHeight;

    protected internal virtual NativeWindowSettings NativeSettings => NativeWindowSettings.Default;
    
    protected internal virtual void registerEngineSystems() { }
    
    // NOTE: Anything bound to GameWindows functions are called first, as evident by the base."function"() calls
    //         . This may change in the future, but most likely will not (you shouldn't be binding to things like UpdateFrame or RenderFrame)
    
    protected internal virtual void initialize() { }
    protected internal virtual void start()      { }
    
    protected internal virtual void unload()     { }
    protected internal virtual void destroy()    { }
 
    protected internal virtual void update()      { }
    protected internal virtual void lateUpdate()  { }
    protected internal virtual void fixedUpdate() { }
 
    protected internal virtual void preRender()  { }
    protected internal virtual void render()     { }
    protected internal virtual void postRender() { } // Called before EngineSystem postRender and RenderManager postRender

    // Events (All events registered in this class are called FIRST [such as onResize being called before RenderManager's onResize])
    protected internal virtual void onResize      (int width, int height) { }
    protected internal virtual void onFocusChange (bool focused) { }
    
    
    protected static void registerEngineSystem<T>() where T : EngineSystem, new()             => EngineCore.registerEngineSystem<T>();
    protected static void registerEngineSystem<T>(EngineSystem system) where T : EngineSystem => EngineCore.registerEngineSystem(system);
}