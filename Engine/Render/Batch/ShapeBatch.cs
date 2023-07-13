using System.Diagnostics;
using System.Reflection;
using Engine.Render.Texture;
using OpenTK.Mathematics;
using BufferUsageHint = OpenTK.Graphics.OpenGL.BufferUsageHint;
using DrawElementsType = OpenTK.Graphics.OpenGL.DrawElementsType;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace Engine.Render.Batch; 

public sealed class ShapeBatch : RenderBatch<ShapeBatch.ShapeVertex> {
    private static readonly int[]     texture_ids = { 0, 1, 2, 3, 4, 5, 6, 7 };
    private static readonly Vector4[] vertex_data = {
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(-0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f, -0.5f, 0.0f, 1.0f)
    };
    private static readonly Vector2[] vertex_uvs = {
        new(0.0f, 0.0f),
        new(0.0f, 1.0f),
        new(1.0f, 1.0f),
        new(1.0f, 0.0f),
    };
    
    private uint                 next_texture;
    private readonly Texture2D[] textures;


    public ShapeBatch(uint vertex_count = 400, uint index_count = 600) : base(vertex_count, index_count) {
        textures = new Texture2D[8];
    }
    
    public void drawBox(TextureAtlas.SubTexture texture, Vector2 position,  Color4 tint, float rotation, float layer = 0.0f, Vector2? scale = null!) => drawBox(position, tint, rotation, layer, scale, texture.Owner.Texture, texture.UVs);
    public void drawBox(TextureAtlas.SubTexture texture, Matrix4 transform, Color4 tint) => drawBox(transform, tint, texture.Owner.Texture, texture.UVs);
    public void drawBox(TextureAtlas.SubTexture texture, Vector2 position,  Color4 tint) => drawBox(position, tint, texture.Owner.Texture, texture.UVs);

    public void drawBox(Vector2 position,  Color4 color, float rotation, float layer = 0.0f, Vector2? scale = null, Texture2D? texture = null, Vector2[]? uvs = null) {
        scale ??= Vector2.One;

        var matrix = Matrix4.CreateScale(scale.Value.X, scale.Value.Y, 1.0f) *
                     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
                     Matrix4.CreateTranslation(position.X, position.Y, layer);
        drawBox(matrix, color, texture, uvs);
    }
    public void drawBox(Matrix4 transform, Color4 color, Texture2D? texture = null, Vector2[]? uvs = null) {
        verifySpace(4, 6);

        var texture_id = getTextureID(texture);

        uvs ??= vertex_uvs;
        
        Indices[IndexCount + 0] = VertexCount + 0;
        Indices[IndexCount + 1] = VertexCount + 1;
        Indices[IndexCount + 2] = VertexCount + 2;
        Indices[IndexCount + 3] = VertexCount + 0;
        Indices[IndexCount + 4] = VertexCount + 2;
        Indices[IndexCount + 5] = VertexCount + 3;

        Vertices[VertexCount + 0].Position  = vertex_data[0] * transform;
        Vertices[VertexCount + 0].Color     = color;
        Vertices[VertexCount + 0].UV        = uvs[0];
        Vertices[VertexCount + 0].TextureID = texture_id;
        Vertices[VertexCount + 1].Position  = vertex_data[1] * transform;
        Vertices[VertexCount + 1].Color     = color;
        Vertices[VertexCount + 1].UV        = uvs[1];
        Vertices[VertexCount + 1].TextureID = texture_id;
        Vertices[VertexCount + 2].Position  = vertex_data[2] * transform;
        Vertices[VertexCount + 2].Color     = color;
        Vertices[VertexCount + 2].UV        = uvs[2];
        Vertices[VertexCount + 2].TextureID = texture_id;
        Vertices[VertexCount + 3].Position  = vertex_data[3] * transform;
        Vertices[VertexCount + 3].Color     = color;
        Vertices[VertexCount + 3].UV        = uvs[3];
        Vertices[VertexCount + 3].TextureID = texture_id;
        
        incrementRawCounts(4, 6);
    }
    public void drawBox(Vector2 position,  Color4 color, Texture2D? texture = null, Vector2[]? uvs = null) {
        verifySpace(4, 6);

        var texture_id = getTextureID(texture);
        var pos = new Vector4(position.X, position.Y, 0.0f, 0.0f);

        uvs ??= vertex_uvs;
        
        Indices[IndexCount + 0] = VertexCount + 0;
        Indices[IndexCount + 1] = VertexCount + 1;
        Indices[IndexCount + 2] = VertexCount + 2;
        Indices[IndexCount + 3] = VertexCount + 0;
        Indices[IndexCount + 4] = VertexCount + 2;
        Indices[IndexCount + 5] = VertexCount + 3;
        
        Vertices[VertexCount + 0].Position  = vertex_data[0] + pos;
        Vertices[VertexCount + 0].Color     = color;
        Vertices[VertexCount + 0].UV        = uvs[0];
        Vertices[VertexCount + 0].TextureID = texture_id;
        Vertices[VertexCount + 1].Position  = vertex_data[1] + pos;
        Vertices[VertexCount + 1].Color     = color;
        Vertices[VertexCount + 1].UV        = uvs[1];
        Vertices[VertexCount + 1].TextureID = texture_id;
        Vertices[VertexCount + 2].Position  = vertex_data[2] + pos;
        Vertices[VertexCount + 2].Color     = color;
        Vertices[VertexCount + 2].UV        = uvs[2];
        Vertices[VertexCount + 2].TextureID = texture_id;
        Vertices[VertexCount + 3].Position  = vertex_data[3] + pos;
        Vertices[VertexCount + 3].Color     = color;
        Vertices[VertexCount + 3].UV        = uvs[3];
        Vertices[VertexCount + 3].TextureID = texture_id;

        incrementRawCounts(4, 6);
    }

    private int getTextureID(Texture2D? texture) {
        if (texture == null)
            return -1;
        
        if (textures.Contains(texture))
            return Array.IndexOf(textures, texture);

        if (next_texture >= 8) {
            next_texture = 0;
            flush();
        }

        textures[next_texture] = texture;
        next_texture++;
        return (int) next_texture - 1;
    }

    protected override void bindResources() {
        for (var i = 0; i < next_texture; i++)
            textures[i].bindTexture(i);
    }

    protected override void bindShaderData() {
        Shader.setUniformArray("uTextures", texture_ids);
    }

    public struct ShapeVertex {
        public Vector4 Position  { get; set; }
        public Color4  Color     { get; set; }
        public Vector2 UV        { get; set; }
        public float   TextureID { get; set; }
    }

    protected override VertexArray.VertexArrayInfo getVertexInfo() {
        var info = new VertexArray.VertexArrayInfo();
        info.pushAttribute<Vector4> (1, false);
        info.pushAttribute<Color4>  (1, false);
        info.pushAttribute<Vector2> (1, false);
        info.pushAttribute<float>   (1, false);

        return info;
    }

    protected override Shader getShader() => RenderManager.getOrCreateShader("ShapeBatch", "Resource.Shader.ShapeBatch.glsl", Engine.EngineAssembly);
}