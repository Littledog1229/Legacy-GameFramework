using OpenTK.Graphics.OpenGL;

namespace Engine.Render; 

public abstract class GLObject {
    protected string ProtectedResourceName { get; set; } = string.Empty;
    
    public int            ObjectHandle { get; protected set; }
    public virtual string ResourceName {
        get => ProtectedResourceName;
        set => trySetResourceName(value);
    }
    
    public abstract ObjectLabelIdentifier LabelIdentifier { get; }
    
    internal abstract void destroy();
    
    protected void trySetResourceName(string value) {
        if (value == ProtectedResourceName)
            return;

        ProtectedResourceName = value;

        if (ProtectedResourceName != string.Empty)
            GL.ObjectLabel(LabelIdentifier, ObjectHandle, ProtectedResourceName.Length, ProtectedResourceName);
    }
}