namespace Engine.Scenes; 

// TODO: Use an ECS system using one of the following libraries
//  . https://github.com/RabbitStewDio/EnTTSharp
//  . https://github.com/sebas77/Svelto.ECS
//  . https://github.com/sschmid/Entitas
//  . https://github.com/genaray/Arch    // (I think this is the one im learning towards right this moment)

// Right now, im learning toward Arch since its newer and may provide much more
// It doesn't really seem to be built for Unity, which is great because it may make use of more optimized C#


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