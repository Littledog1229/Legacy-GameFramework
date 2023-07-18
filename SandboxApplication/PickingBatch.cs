using ApplicationCore.Render;
using ApplicationCore.Render.Batch;
using OpenTK.Mathematics;

namespace Sandbox; 

public class PickingBatch : RenderBatch<PickingBatch.PickingVertex> {
    private static readonly Vector4[] vertex_data = {
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(-0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f, -0.5f, 0.0f, 1.0f)
    };

    public PickingBatch(uint vertex_count = 400, uint index_count = 600) : base(vertex_count, index_count) { }
    public PickingBatch(ShapeBatch batch) : base(batch.MaxVertices, batch.MaxIndices) { }
    
    public void drawBox(uint batch_index, Vector2 position, float rotation, float layer = 0.0f, Vector2? scale = null) {
        scale ??= Vector2.One;

        var matrix = Matrix4.CreateScale(scale.Value.X, scale.Value.Y, 1.0f) *
                     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
                     Matrix4.CreateTranslation(position.X, position.Y, layer);
        drawBox(batch_index, matrix);
    }
    public void drawBox(uint batch_index, Matrix4 transform) {
        verifySpace(4, 6); ;
        
        Indices[IndexCount + 0] = VertexCount + 0;
        Indices[IndexCount + 1] = VertexCount + 1;
        Indices[IndexCount + 2] = VertexCount + 2;
        Indices[IndexCount + 3] = VertexCount + 0;
        Indices[IndexCount + 4] = VertexCount + 2;
        Indices[IndexCount + 5] = VertexCount + 3;

        Vertices[VertexCount + 0].Position  = vertex_data[0] * transform;
        Vertices[VertexCount + 0].Index     = batch_index;
        Vertices[VertexCount + 1].Position  = vertex_data[1] * transform;
        Vertices[VertexCount + 1].Index     = batch_index;
        Vertices[VertexCount + 2].Position  = vertex_data[2] * transform;
        Vertices[VertexCount + 2].Index     = batch_index;
        Vertices[VertexCount + 3].Position  = vertex_data[3] * transform;
        Vertices[VertexCount + 3].Index     = batch_index;
        
        incrementRawCounts(4, 6);
    }
    public void drawBox(uint batch_index, Vector2 position) {
        verifySpace(4, 6);

        var pos = new Vector4(position.X, position.Y, 0.0f, 0.0f);

        Indices[IndexCount + 0] = VertexCount + 0;
        Indices[IndexCount + 1] = VertexCount + 1;
        Indices[IndexCount + 2] = VertexCount + 2;
        Indices[IndexCount + 3] = VertexCount + 0;
        Indices[IndexCount + 4] = VertexCount + 2;
        Indices[IndexCount + 5] = VertexCount + 3;
        
        Vertices[VertexCount + 0].Position  = vertex_data[0] + pos;
        Vertices[VertexCount + 0].Index     = batch_index;
        Vertices[VertexCount + 1].Position  = vertex_data[1] + pos;
        Vertices[VertexCount + 1].Index     = batch_index;
        Vertices[VertexCount + 2].Position  = vertex_data[2] + pos;
        Vertices[VertexCount + 2].Index     = batch_index;
        Vertices[VertexCount + 3].Position  = vertex_data[3] + pos;
        Vertices[VertexCount + 3].Index     = batch_index;

        incrementRawCounts(4, 6);
    }
    
    public struct PickingVertex {
        public Vector4 Position { get; set; }
        public uint    Index    { get; set; }
    }

    protected override VertexArray.VertexArrayInfo getVertexInfo() {
        var info = new VertexArray.VertexArrayInfo();
        
        info.pushAttribute<Vector4>(1, false);
        info.pushAttribute<uint>(1, false);
        
        return info;
    }

    protected override Shader getShader() => RenderManager.getOrCreateShader("PickingBatch", "Resource.Shader.PickingBatch.glsl", null);
}