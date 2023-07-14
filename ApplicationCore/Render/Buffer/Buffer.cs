using OpenTK.Graphics.OpenGL;

namespace ApplicationCore.Render.Buffer; 

public abstract class Buffer : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.Buffer;

    protected Buffer() { ObjectHandle = GL.GenBuffer(); }
    
    public abstract void bind();
    public abstract void unbind(); // Not static due to the nature of inheritance

    public override void destroy() {
        GL.DeleteBuffer(ObjectHandle);
        deregister();
    }
    
    internal void internalDestroy() => GL.DeleteBuffer(ObjectHandle);

    protected abstract void deregister();
}