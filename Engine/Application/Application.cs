using OpenTK.Windowing.Desktop;

namespace Engine.Application; 

public abstract class Application {
    protected internal virtual GameWindowSettings   GameSettings   => GameWindowSettings.Default;
    protected internal virtual NativeWindowSettings NativeSettings => NativeWindowSettings.Default;
    
    protected internal virtual void initialize() { }
    protected internal virtual void start()      { }
    
    protected internal virtual void onUpdate()      { }
    protected internal virtual void onFixedUpdate() { }
    protected internal virtual void onRender()      { }
    
    protected internal virtual void destroy() { }
    
    protected internal virtual void onResize() { }
    protected internal virtual void onFocusGain() { }
    protected internal virtual void onFocusLost() { }
}