using OpenTK.Graphics.OpenGL;

namespace Engine.Render.Buffer; 

public abstract class Buffer : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.Buffer;

    internal Buffer() { ObjectHandle = GL.GenBuffer(); }
    
    public abstract void bind();
    public abstract void unbind(); // Not static due to the nature of inheritance

    internal override void destroy() => GL.DeleteBuffer(ObjectHandle);
}