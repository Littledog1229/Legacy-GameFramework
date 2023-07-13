using Engine.Render;
using Engine.Render.Camera;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpMinecraft.Block;

namespace SharpMinecraft.World; 

public sealed class ChunkRenderer {
    private Chunk       chunk;
    private VertexArray array;

    private int element_count = 0;

    public ChunkRenderer(Chunk chunk) {
        this.chunk = chunk;
        chunk.ChunkUpdated += _ => rebuildChunk();

        {
            var info = new VertexArray.VertexArrayInfo();
            info.pushAttribute<Vector3> (1, false);
            info.pushAttribute<Color4>  (1, false);
            info.pushAttribute<Vector2> (1, false);

            array = RenderManager.createVertexArray(ref info);
        }

        rebuildChunk();
    }

    private void rebuildChunk() {
        var block_model = BlockModel.DEFAULT;

        var vertices = new List<TerrainVertex>(400000);
        var indices  = new List<uint>(400000);

        var chunk_world_position = chunk.Position.toWorldPosition();
        var world = chunk.World;

        for (var z = 0u; z < Chunk.CHUNK_SIZE_Z; z++) {
            for (var y = 0u; y < Chunk.CHUNK_SIZE_Y; y++) {
                for (var x = 0u; x < Chunk.CHUNK_SIZE_X; x++) {
                    var block = BlockRegistry.getBlock((int)chunk[x, y, z]);
                    var position = new Vector3(x, y, z) + chunk_world_position;

                    var construct_faces = 0b000000;

                    // Front Face
                    if (!blockSolid(x + 1, y, z))
                        construct_faces |= 0b100000;
                    
                    // Back Face
                    if (!blockSolid(x - 1, y, z))
                        construct_faces |= 0b010000;
                    
                    // Right Face
                    if (!blockSolid(x, y, z + 1))
                        construct_faces |= 0b001000;
                    
                    // Left Face
                    if (!blockSolid(x, y, z - 1))
                        construct_faces |= 0b000100;

                    // Top Face
                    if (!blockSolid(x, y + 1, z))
                        construct_faces |= 0b000010;
                    
                    // Bottom Face
                    if (!blockSolid(x, y - 1, z))
                        construct_faces |= 0b000001;

                    // Front Face
                    if ((construct_faces & 0b100000) > 0)
                        buildFace(block, block_model.FrontFace, position, vertices, indices);

                    // Back Face
                    if ((construct_faces & 0b010000) > 0)
                        buildFace(block, block_model.BackFace, position, vertices, indices);

                    // Right Face
                    if ((construct_faces & 0b001000) > 0)
                        buildFace(block, block_model.RightFace, position, vertices, indices);

                    // Left Face
                    if ((construct_faces & 0b000100) > 0)
                        buildFace(block, block_model.LeftFace, position, vertices, indices);

                    // Top Face
                    if ((construct_faces & 0b000010) > 0)
                        buildFace(block, block_model.TopFace, position, vertices, indices);

                    // Bottom Face
                    if ((construct_faces & 0b000001) > 0)
                        buildFace(block, block_model.BottomFace, position, vertices, indices);
                }
            }
        }

        array.VertexBuffer.bufferData(vertices.ToArray());
        array.IndexBuffer.bufferData(indices.ToArray());
        
        element_count = indices.Count;
        Console.WriteLine(element_count);
    }

    public void render(Camera camera) {
        array.bind();

        TestChunkRenderManager.TerrainShader.bind();
        TestChunkRenderManager.TerrainShader.setUniform("uProjection", camera.Projection);
        TestChunkRenderManager.TerrainShader.setUniform("uView",       camera.View);
        TestChunkRenderManager.TerrainShader.setUniform("uTexture",    0);
        
        BlockAtlas.Texture.bindTexture(0);
        
        array.drawElements(PrimitiveType.Triangles, element_count, DrawElementsType.UnsignedInt, 0);
    }

    public void destroy() => RenderManager.destroyVertexArray(array);

    private bool blockSolid(uint x, uint y, uint z) {
        return !BlockRegistry.getBlock((int) chunk.World.getBlockRelative(new Vector3i((int)x, (int)y, (int)z), chunk.Position)).Transparent;

        return !BlockRegistry.getBlock((int)chunk[x, y, z]).Transparent;
    }   
    
    private void buildFace(Block.Block block, BlockModel.FaceInformation? block_face, Vector3 position_offset, List<TerrainVertex> vertices, List<uint> indices) {
        if (block_face == null)
            return;
        
        indices.AddRange(block_face.Value.Indices.Select(index => index + (uint)vertices.Count));
        vertices.AddRange(block_face.Value.Vertices.Select(vertex => new TerrainVertex {
            Position = position_offset + vertex.Position, 
            Color    = Color4.White,
            UV       = BlockAtlas.getUV((int) block.Textures[vertex.TextureID], vertex.UV)
        }));
    }

    private readonly struct TerrainVertex {
        public Vector3 Position { get; init; }
        public Color4  Color    { get; init; }
        public Vector2 UV       { get; init; }
    }
}