using Engine.Utility;
using OpenTK.Graphics.OpenGL;

namespace ApplicationCore.Render.Buffer; 

public sealed class VertexBuffer : Buffer {
    public override void bind()   => GL.BindBuffer(BufferTarget.ArrayBuffer, ObjectHandle);
    public override void unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    public VertexBuffer() { RenderManager.registerVertexBuffer(this); Console.WriteLine(ObjectHandle); }
    
    public void bufferData(float[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
        bind();
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, hint);
    }
    public void bufferData<T>(T[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct {
        bind();
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * SizeUtility.getTypeSize<T>(), data, hint);
    }
    public void bufferData<T>(T[] data, int size, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct {
        bind();
        GL.BufferData(BufferTarget.ArrayBuffer, size * SizeUtility.getTypeSize<T>(), data, hint);
        
    }
    
    public void bufferSubData<T>(T[] data, int offset, int size) where T : struct {
        bind();
        GL.BufferSubData(BufferTarget.ArrayBuffer, offset, size * SizeUtility.getTypeSize<T>(), data);
        
#if DEBUG
        unbind(); // Nice for debugging, it ensures that if we dont specifically bind a buffer then it will throw an error.
#endif
    }
    
    protected override void deregister() => RenderManager.deregisterVertexBuffer(this);
}