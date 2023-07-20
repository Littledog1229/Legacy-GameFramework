namespace Sandbox.Editor; 

public abstract class CreationContext {
    public abstract void   reset();
    public abstract void   renderImgui();
    public abstract object create();

    public T create<T>() where T : class => (create() as T)!;
}