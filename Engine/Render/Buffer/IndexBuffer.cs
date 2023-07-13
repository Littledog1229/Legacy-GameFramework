using Engine.Utility;
using OpenTK.Graphics.OpenGL;

namespace Engine.Render.Buffer; 

public sealed class IndexBuffer : Buffer {
    public override void bind()   => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ObjectHandle);
    public override void unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

    public void bufferData<T>(T[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : unmanaged {
        bind();
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * SizeUtility.getTypeSize<T>(), data, hint);
    }
    
    public void bufferData<T>(T[] data, int size, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : unmanaged {
        bind();
        GL.BufferData(BufferTarget.ElementArrayBuffer, size * SizeUtility.getTypeSize<T>(), data, hint);
    }

    public void bufferSubData(uint[] data, int offset, int count) {
        bind();
        GL.BufferSubData(BufferTarget.ElementArrayBuffer, offset, count, data);
    }
}