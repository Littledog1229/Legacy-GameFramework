namespace Engine.Scenes; 

public abstract class Scene {
    public virtual void start()  { }
    public virtual void unload() { }
    
    public virtual void destroy() { }
    
    public virtual void update()      { }
    public virtual void fixedUpdate() { }
    public virtual void lateUpdate()  { }
    
    public virtual void preRender()  { }
    public virtual void render()     { }
    public virtual void postRender() { }
}