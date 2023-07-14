namespace Engine; 

// TODO: Dependencies      through Attributes
// TODO: System Priorities through Attributes
public abstract class EngineSystem {
    protected internal virtual void initialize() { } // Used to setup anything specific for this system, do not initialize dependencies in this function
    protected internal virtual void start()      { } // Called after system initialization,   used for getting resources and dependencies
    protected internal virtual void unload()     { } // Called before the final shutdown,     used for releasing resources from other systems
    protected internal virtual void destroy()    { } // Called before the application closes, used for destroying resources specific to this system
    
    protected internal virtual void update()      { } // Called during the update phase of the application, use Time.DeltaTime to get the delta time
    protected internal virtual void lateUpdate()  { } // Called last (before rendering), primarily used for finalizing state (such as updating input states for the next frame)
    protected internal virtual void fixedUpdate() { } // Called (potentially) at the very beginning of the update frame, used for updating physics (and can happen multiple times before any updates)
    
    protected internal virtual void preRender()  { } // Called right before the current frame is rendered, used for updating framebuffer states
    protected internal virtual void render()     { } // Called during the render phase of the application, used for rendering things to the screen (or other framebuffers)
    protected internal virtual void postRender() { } // Called after all rendering is concluded, and before the renderer swaps the main framebuffer
}