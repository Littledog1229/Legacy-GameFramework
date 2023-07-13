using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Engine.Render; 

public abstract class RenderBatch<T> where T : struct {
    public Shader Shader { get; private set; } = null!;
    
    public uint MaxVertices { get; }
    public uint MaxIndices  { get; }
    
    protected uint VertexCount { get; private set; }
    protected uint IndexCount  { get; private set; }

    // Null only when the batch has ended and not begun
    protected Camera.Camera CurrentCamera { get; private set; } = null!;

    private VertexArray vertex_array = null!;

    protected readonly T[]    Vertices;
    protected readonly uint[] Indices;

#if DEBUG
    private bool started;
#endif

    protected abstract VertexArray.VertexArrayInfo getVertexInfo();
    protected abstract Shader                      getShader();

    public RenderBatch(uint max_vertex_count = 400, uint max_index_count = 600) {
        MaxVertices = max_vertex_count;
        MaxIndices  = max_index_count;

        Vertices = new T   [MaxVertices];
        Indices  = new uint[MaxIndices];
    }

    public void initialize() {
        Shader = getShader();
        
        {
            var info = getVertexInfo();

            vertex_array = RenderManager.createVertexArray(ref info);
        }
    }
    public void begin(Camera.Camera camera) {
#if DEBUG
        if (started)
            throw new Exception("Batch had already been started!");
        started = true;
#endif

        CurrentCamera = camera;
    }
    public void end() {
        flush();

        CurrentCamera = null!;
#if DEBUG
        if (!started)
            throw new Exception("Batch had already been ended!");
        started = false;
#endif
    }
    
    protected virtual void bindShaderData() { }
    protected virtual void bindResources()  { }
    protected virtual void resetBatch()     { }

    protected void verifySpace(uint vertices, uint indices) {
        if (VertexCount + vertices > MaxVertices && IndexCount + indices > MaxIndices)
            flush();
        
        Debug.Assert(vertices <= MaxVertices, "The amount of requested vertices is greater than the max vertices this batch provides!");
        Debug.Assert(indices  <= MaxIndices,  "The amount of requested indices is greater than the max indices this batch provides!");
    }
    protected void incrementRawCounts(uint vertices, uint indices) {
        VertexCount += vertices;
        IndexCount += indices;
    }

    protected void flush() {
        vertex_array.VertexBuffer.bufferData (Vertices, (int) VertexCount, BufferUsageHint.DynamicDraw);
        vertex_array.IndexBuffer.bufferData  (Indices,  (int) IndexCount,  BufferUsageHint.DynamicDraw);

        vertex_array.bind();

        Shader.bind();
        Shader.setUniform("uProjection", CurrentCamera.Projection);
        Shader.setUniform("uView",       CurrentCamera.View);
        
        bindShaderData();
        bindResources();
        
        vertex_array.drawElements(PrimitiveType.Triangles, (int) IndexCount, DrawElementsType.UnsignedInt, 0);

        VertexCount = 0;
        IndexCount  = 0;
        resetBatch();
        
#if DEBUG
        VertexArray.unbind();
#endif
    }
}