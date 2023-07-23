using ApplicationCore.Render.Buffer;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using NativeVector2 = System.Numerics.Vector2;
using NativeVector3 = System.Numerics.Vector3;
using NativeVector4 = System.Numerics.Vector4;

namespace ApplicationCore.Render; 

public sealed class VertexArray : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.VertexArray;

    public override string ResourceName {
        get => ProtectedResourceName;
        set {
            trySetResourceName(value);
            VertexBuffer.ResourceName = $"{value}.vertex_buffer";
            IndexBuffer.ResourceName  = $"{value}.index_buffer";
        }
    }

    public struct VertexArrayInfo {
        public VertexArrayInfo() { }
        
        public void pushAttribute<T>(int size, bool normalized) {
            var info             = TYPE_INFO[typeof(T)];
            var element_size = (int) info.Size * size;
            
            Elements.Add(new VertexArrayElement() {
                Index      = next_index,
                Offset     = next_offset,
                Size       = element_size,
                Type       = info.Type,
                Normalized = normalized
            });

            next_index++;
            next_offset += (int) info.ByteSize * size;
            
            Stride = next_offset;
        }

        private readonly struct TypeInfo {
            public VertexAttribPointerType Type     { get; }
            public uint                    Size     { get; }
            public uint                    ByteSize { get; }

            public TypeInfo(VertexAttribPointerType type, uint size, uint byte_size) {
                Type       = type;
                Size       = size;
                ByteSize   = byte_size;
            }
        }
        
        internal readonly struct VertexArrayElement {
            public VertexAttribPointerType Type       { get; init; }
            public int                     Size       { get; init; }
            public int                     Index      { get; init; }
            public int                     Offset     { get; init; }
            public bool                    Normalized { get; init; }
        }

        private static readonly Dictionary<Type, TypeInfo> TYPE_INFO = new() {
            { typeof(sbyte),   new TypeInfo(VertexAttribPointerType.Byte,   1, 1) },
            { typeof(short),   new TypeInfo(VertexAttribPointerType.Short,  1, 2) },
            { typeof(int),     new TypeInfo(VertexAttribPointerType.Int,    1, 4) },
            { typeof(float),   new TypeInfo(VertexAttribPointerType.Float,  1, 4) },
            { typeof(double),  new TypeInfo(VertexAttribPointerType.Double, 1, 8) },
            
            { typeof(byte),   new TypeInfo(VertexAttribPointerType.UnsignedByte,  1, 1) },
            { typeof(ushort), new TypeInfo(VertexAttribPointerType.UnsignedShort, 1, 2) },
            { typeof(uint),   new TypeInfo(VertexAttribPointerType.UnsignedInt,   1, 4) },
            
            // OpenTK Mathematics
            { typeof(Vector2),       new TypeInfo(VertexAttribPointerType.Float, 2, 4 * 2) },
            { typeof(Vector3),       new TypeInfo(VertexAttribPointerType.Float, 3, 4 * 3) },
            { typeof(Vector4),       new TypeInfo(VertexAttribPointerType.Float, 4, 4 * 4) },
            { typeof(Color4),        new TypeInfo(VertexAttribPointerType.Float, 4, 4 * 4) },
            
            // System.Numerics
            { typeof(NativeVector2), new TypeInfo(VertexAttribPointerType.Float, 2, 4 * 2) },
            { typeof(NativeVector3), new TypeInfo(VertexAttribPointerType.Float, 3, 4 * 3) },
            { typeof(NativeVector4), new TypeInfo(VertexAttribPointerType.Float, 4, 4 * 4) }
    };

        private int next_index  = 0;
        private int next_offset = 0;

        internal List<VertexArrayElement> Elements { get; private set; } = new();
        internal int                      Stride   { get; private set; }
    }
    
    public VertexBuffer VertexBuffer { get; }
    public IndexBuffer  IndexBuffer  { get; }

    public override void destroy() {
        GL.DeleteVertexArray(ObjectHandle);
        RenderManager.deregisterVertexArray(this);
        
        VertexBuffer.destroy();
        IndexBuffer.destroy();
    }

    public void        bind()   => GL.BindVertexArray(ObjectHandle);
    public static void unbind() => GL.BindVertexArray(0);
    
    public void drawElements(PrimitiveType mode, int count, DrawElementsType type, int indices) => GL.DrawElements(mode, count, type, indices);

    public VertexArray(ref VertexArrayInfo info) {
        ObjectHandle = GL.GenVertexArray();
        bind();

        VertexBuffer = new VertexBuffer();
        IndexBuffer  = new IndexBuffer();
        
        VertexBuffer.bind();

        foreach (var element in info.Elements) {
            var offset = element.Offset;
            
            switch (element.Type) {
                case VertexAttribPointerType.Int:
                    GL.VertexAttribIPointer(element.Index, element.Size, VertexAttribIntegerType.Int, info.Stride, new IntPtr(offset));
                    break;
                case VertexAttribPointerType.UnsignedInt:
                    
                    GL.VertexAttribIPointer(element.Index, element.Size, VertexAttribIntegerType.UnsignedInt, info.Stride, new IntPtr(offset));
                    break;
                default:
                    GL.VertexAttribPointer(element.Index, element.Size, element.Type, element.Normalized, info.Stride, element.Offset);
                    break;
            }
            
            GL.EnableVertexAttribArray(element.Index);
        }
        
        IndexBuffer.bind();

        unbind();
        
        RenderManager.registerVertexArray(this);
    }

    internal void internalDestroy() {
        GL.DeleteVertexArray(ObjectHandle);
        
        VertexBuffer.internalDestroy();
        IndexBuffer.internalDestroy();
    }
}