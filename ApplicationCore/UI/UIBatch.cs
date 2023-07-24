using ApplicationCore.Render;
using ApplicationCore.Render.Batch;
using ApplicationCore.UI;
using OpenTK.Mathematics;

namespace Engine.Render.Batch; 

public sealed class UIBatch : RenderBatch<UIBatch.UIVertex> {
    public struct UIVertex {
        public Vector2 Position { get; set; }
        public Color4  Color    { get; set; }
    }

    public void drawTransform(UITransform transform, Color4 tint) {
        verifySpace(4, 6);
        
        var min = transform.TrueMin;
        var max = transform.TrueMax;

        Indices[IndexCount + 0] = VertexCount + 0;
        Indices[IndexCount + 1] = VertexCount + 1;
        Indices[IndexCount + 2] = VertexCount + 2;
        Indices[IndexCount + 3] = VertexCount + 0;
        Indices[IndexCount + 4] = VertexCount + 2;
        Indices[IndexCount + 5] = VertexCount + 3;

        Vertices[VertexCount + 0].Position = min;
        Vertices[VertexCount + 1].Position = new Vector2(min.X, max.Y);
        Vertices[VertexCount + 2].Position = max;
        Vertices[VertexCount + 3].Position = new Vector2(max.X, min.Y);
        
        Vertices[VertexCount + 0].Color = tint;
        Vertices[VertexCount + 1].Color = tint;
        Vertices[VertexCount + 2].Color = tint;
        Vertices[VertexCount + 3].Color = tint;
        
        incrementRawCounts(4, 6);
    }
    
    protected override VertexArray.VertexArrayInfo getVertexInfo() {
        var array = new VertexArray.VertexArrayInfo();
        array.pushAttribute<Vector2>(1, false);
        array.pushAttribute<Color4> (1, false);

        return array;
    }

    protected override Shader getShader() => RenderManager.getOrCreateShader("UIBatch", "Resource.Shader.UIShader.glsl", EngineCore.EngineResourceAssembly);
}