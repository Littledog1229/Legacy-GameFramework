using Engine.Render;
using Engine.Render.Camera;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpMinecraft.Block;

namespace SharpMinecraft.World; 

// TODO: Swap to disposable pattern
public sealed class BatchedChunkRenderer {
    public static Shader TerrainShader { get; private set; } = null!;

    private VertexArray.VertexArrayInfo info;

    // List of chunks that need to be built or rebuilt on the chunk builder thread
    private readonly List<Chunk> chunks_to_build      = new();
    private readonly object      chunks_to_build_lock = new();
    //private readonly List<Chunk> chunks_to_destroy = new();
    private readonly Thread      chunk_builder_thread;
    //private readonly VertexArray vertex_array;
    private readonly World       world;
    
    private bool builder_active = true;
    //private bool rebuffer_data;

    //private readonly Dictionary<Chunk, ChunkBatchData> batch_data  = new();
    //private readonly List<TerrainVertex>               vertex_data = new();
    //private readonly List<uint>                        index_data  = new();

    private readonly object data_buffer_lock = new();
    
    private readonly Dictionary<Chunk, VertexArray>               arrays         = new();
    private readonly Dictionary<Chunk, (TerrainVertex[], uint[])> data_to_buffer = new();
    private readonly Dictionary<VertexArray, int>                 element_counts = new();

    //private int element_count;

    public BatchedChunkRenderer(World world) {
        if (TerrainShader == null!)
            TerrainShader = RenderManager.getOrCreateShader("terrain", "Resource.Shader.Terrain.glsl");

        this.world = world;
        
        chunk_builder_thread = new Thread(buildChunks) { Name = "ChunkBuilderThread" };
        chunk_builder_thread.Start();

        info = new VertexArray.VertexArrayInfo();
        info.pushAttribute<Vector3> (1, false);
        info.pushAttribute<Color4>  (1, false);
        info.pushAttribute<Vector2> (1, false);
        
        /*{
            var info = new VertexArray.VertexArrayInfo();
            info.pushAttribute<Vector3> (1, false);
            info.pushAttribute<Color4>  (1, false);
            info.pushAttribute<Vector2> (1, false);

            vertex_array = RenderManager.createVertexArray(ref info);
        }*/

        world.OnChunkLoad   += addChunk;
        world.OnChunkUnload += removeChunk;
    }

    private void removeChunk(Chunk chunk) {
        //chunks_to_destroy.Add(chunk);
        if (!arrays.ContainsKey(chunk))
            return;

        lock (chunks_to_build_lock) {
            RenderManager.destroyVertexArray(arrays[chunk]);
            arrays.Remove(chunk);
            chunks_to_build.Remove(chunk);
        }
    }

    private void addChunk(Chunk chunk) {
        arrays.Add(chunk, RenderManager.createVertexArray(ref info));
        element_counts.Add(arrays[chunk], 0);

        lock (chunks_to_build_lock) {
            if (!chunks_to_build.Contains(chunk))
                chunks_to_build.Add(chunk);

            /*var pos = chunk.Position;
            if (world.tryGetChunk(pos + new ChunkPosition(-1,  0), out var update) && !chunks_to_build.Contains(update!)) {
                chunks_to_build.Add(update!);
                data_to_buffer.Remove(update!);
            }
            if (world.tryGetChunk(pos + new ChunkPosition(1, 0), out update) && !chunks_to_build.Contains(update!)) {
                chunks_to_build.Add(update!);
                data_to_buffer.Remove(update!);
            }
            if (world.tryGetChunk(pos + new ChunkPosition( 0, -1), out update) && !chunks_to_build.Contains(update!)) {
                chunks_to_build.Add(update!);
                data_to_buffer.Remove(update!);
            }
            if (world.tryGetChunk(pos + new ChunkPosition( 0,  1), out update) && !chunks_to_build.Contains(update!)) {
                chunks_to_build.Add(update!);
                data_to_buffer.Remove(update!);
            }*/
        }
    }

    // TODO: This bottlenecks due to how large the buffers are, maybe allocate a buffer per chunk to fix this problem?
    public void render(Camera camera) {
        lock (data_buffer_lock) {
            if (data_to_buffer.Count > 0) {
                foreach (var (chunk, (vertex, index)) in data_to_buffer) {
                    if (!arrays.ContainsKey(chunk))
                        continue;
                    
                    arrays[chunk].VertexBuffer.bufferData(vertex);
                    arrays[chunk].IndexBuffer.bufferData(index);

                    element_counts[arrays[chunk]] = index.Length;
                }
                
                data_to_buffer.Clear();
            }
        }

            /*if (rebuffer_data) {
                vertex_array.VertexBuffer.bufferData(vertex_data.ToArray(), BufferUsageHint.DynamicDraw);
                vertex_array.IndexBuffer.bufferData(index_data.ToArray(), BufferUsageHint.DynamicDraw);
                
                rebuffer_data = false;
            }*/
        
        TerrainShader.bind();
        TerrainShader.setUniform("uProjection", camera.Projection);
        TerrainShader.setUniform("uView",       camera.View);
        TerrainShader.setUniform("uTexture", 0);
        
        BlockAtlas.Texture.bindTexture(0);

        foreach (var array in arrays.Values) {
            array.bind();
            array.drawElements(PrimitiveType.Triangles, element_counts[array], DrawElementsType.UnsignedInt, 0);
        }
        
        //vertex_array.bind();
        //vertex_array.drawElements(PrimitiveType.Triangles, element_count, DrawElementsType.UnsignedInt, 0);
    }

    public void destroy() {
        builder_active = false;
        chunk_builder_thread.Join();
        
        foreach(var array in arrays.Values)
            RenderManager.destroyVertexArray(array);
        
        arrays.Clear();
        element_counts.Clear();
        
        //RenderManager.destroyVertexArray(vertex_array);
    }

    private readonly List<Chunk> chunks_to_build_clone   = new();
    //private readonly List<Chunk> chunks_to_destroy_clone = new();
    
    private readonly List<TerrainVertex> vertices = new();
    private readonly List<uint>          indices  = new();
    
    private void buildChunks() {
        while (builder_active) {
            Thread.Sleep(1);

            /*// Register chunks to destroy
            lock (chunks_to_destroy) {
                chunks_to_destroy_clone.AddRange(chunks_to_destroy);
                chunks_to_destroy.Clear();
            }*/

            Chunk build_chunk = null!;
            
            // Register chunks to build
            lock (chunks_to_build_lock) {
                if (chunks_to_build.Count <= 0)
                    continue;

                build_chunk = chunks_to_build[0];
                chunks_to_build.Remove(build_chunk);
                //chunks_to_build_clone.AddRange(chunks_to_build);
                //chunks_to_build.Clear();

            }

            /*if (chunks_to_build_clone.Count <= 0/* && chunks_to_destroy_clone.Count <= 0#1#)
                continue;*/

            /*// (int, int) -> index_index, vertex_count_to_remove
            List<(int, int)> indexes_to_update = new();
            
            foreach (var chunk in chunks_to_destroy_clone) {
                if (!batch_data.TryGetValue(chunk, out var data))
                    continue; // There is no batch data for some reason?
                
                indexes_to_update.Add((data.IndexOffset, data.VertexCount));
                vertex_data.RemoveRange(data.VertexOffset, data.VertexCount);
                index_data.RemoveRange(data.IndexOffset, data.IndexCount);

                batch_data.Remove(chunk);
            }*/

            /*// Update indices
            foreach (var (index, count) in indexes_to_update) {
                for (var i = index; i < index_data.Count; i++) {
                    index_data[i] -= (uint) count;
                }
            }*/

            var chunk_position = build_chunk.Position;
                var chunk_world_position = build_chunk.Position.toWorldPosition();
                
                for (var z = 0u; z < Chunk.CHUNK_SIZE_Z; z++) {
                    for (var y = 0u; y < Chunk.CHUNK_SIZE_Y; y++) {
                        for (var x = 0u; x < Chunk.CHUNK_SIZE_X; x++) {
                            var block = BlockRegistry.getBlock((int)build_chunk[x, y, z]);
                            var block_model = block.Model;
                            var position = new Vector3(x, y, z) + chunk_world_position;

                            var construct_faces = 0b000000;
                            
                            // Front Face
                            if (!blockSolid(ref chunk_position, x + 1, y, z))
                                construct_faces |= 0b100000;
                    
                            // Back Face
                            if (!blockSolid(ref chunk_position, x - 1, y, z))
                                construct_faces |= 0b010000;
                    
                            // Right Face
                            if (!blockSolid(ref chunk_position, x, y, z + 1))
                                construct_faces |= 0b001000;
                    
                            // Left Face
                            if (!blockSolid(ref chunk_position, x, y, z - 1))
                                construct_faces |= 0b000100;

                            // Top Face
                            if (!blockSolid(ref chunk_position, x, y + 1, z))
                                construct_faces |= 0b000010;
                    
                            // Bottom Face
                            if (!blockSolid(ref chunk_position, x, y - 1, z))
                                construct_faces |= 0b000001;

                            // Front Face
                            if ((construct_faces & 0b100000) > 0)
                                constructBlockFace(block, block_model.FrontFace, position);

                            // Back Face
                            if ((construct_faces & 0b010000) > 0)
                                constructBlockFace(block, block_model.BackFace, position);

                            // Right Face
                            if ((construct_faces & 0b001000) > 0)
                                constructBlockFace(block, block_model.RightFace, position);

                            // Left Face
                            if ((construct_faces & 0b000100) > 0)
                                constructBlockFace(block, block_model.LeftFace, position);

                            // Top Face
                            if ((construct_faces & 0b000010) > 0)
                                constructBlockFace(block, block_model.TopFace, position);

                            // Bottom Face
                            if ((construct_faces & 0b000001) > 0)
                                constructBlockFace(block, block_model.BottomFace, position);
                        }
                    }
                }

                lock (data_buffer_lock)
                    data_to_buffer.Add(build_chunk, (vertices.ToArray(), indices.ToArray()));

                vertices.Clear();
                indices.Clear();
            
            /*foreach (var chunk in chunks_to_build_clone) {
                //var vertex_start = vertex_data.Count;
                //var index_start  = index_data.Count;

                var chunk_position = chunk.Position;
                var chunk_world_position = chunk.Position.toWorldPosition();
                
                for (var z = 0u; z < Chunk.CHUNK_SIZE_Z; z++) {
                    for (var y = 0u; y < Chunk.CHUNK_SIZE_Y; y++) {
                        for (var x = 0u; x < Chunk.CHUNK_SIZE_X; x++) {
                            var block = BlockRegistry.getBlock((int)chunk[x, y, z]);
                            var block_model = block.Model;
                            var position = new Vector3(x, y, z) + chunk_world_position;

                            var construct_faces = 0b000000;
                            
                            // Front Face
                            if (!blockSolid(ref chunk_position, x + 1, y, z))
                                construct_faces |= 0b100000;
                    
                            // Back Face
                            if (!blockSolid(ref chunk_position, x - 1, y, z))
                                construct_faces |= 0b010000;
                    
                            // Right Face
                            if (!blockSolid(ref chunk_position, x, y, z + 1))
                                construct_faces |= 0b001000;
                    
                            // Left Face
                            if (!blockSolid(ref chunk_position, x, y, z - 1))
                                construct_faces |= 0b000100;

                            // Top Face
                            if (!blockSolid(ref chunk_position, x, y + 1, z))
                                construct_faces |= 0b000010;
                    
                            // Bottom Face
                            if (!blockSolid(ref chunk_position, x, y - 1, z))
                                construct_faces |= 0b000001;

                            // Front Face
                            if ((construct_faces & 0b100000) > 0)
                                constructBlockFace(block, block_model.FrontFace, position);

                            // Back Face
                            if ((construct_faces & 0b010000) > 0)
                                constructBlockFace(block, block_model.BackFace, position);

                            // Right Face
                            if ((construct_faces & 0b001000) > 0)
                                constructBlockFace(block, block_model.RightFace, position);

                            // Left Face
                            if ((construct_faces & 0b000100) > 0)
                                constructBlockFace(block, block_model.LeftFace, position);

                            // Top Face
                            if ((construct_faces & 0b000010) > 0)
                                constructBlockFace(block, block_model.TopFace, position);

                            // Bottom Face
                            if ((construct_faces & 0b000001) > 0)
                                constructBlockFace(block, block_model.BottomFace, position);
                        }
                    }
                }

                //var vertex_count = vertex_data.Count - vertex_start;
                //var index_count  = index_data.Count  - index_start;
                
                //vertex_data.AddRange(vertices);
                //index_data.AddRange(indices);
                
                //batch_data.Add(chunk, new ChunkBatchData(vertex_start, index_start, vertex_count, index_count));

                lock (data_buffer_lock)
                    data_to_buffer.Add(chunk, (vertices.ToArray(), indices.ToArray()));

                vertices.Clear();
                indices.Clear();
            }*/

            //element_count = index_data.Count;
            //rebuffer_data = true;
            
            //chunks_to_destroy_clone.Clear();
            chunks_to_build_clone.Clear();
        }
    }
    
    private bool blockSolid(ref ChunkPosition position, uint x, uint y, uint z) => !BlockRegistry.getBlock((int) world.getBlockRelative(new Vector3i((int)x, (int)y, (int)z), position)).Transparent;

    private void constructBlockFace(Block.Block block, BlockModel.FaceInformation? block_face, Vector3 position_offset) {
        if (block_face == null)
            return;
        
        indices.AddRange(block_face.Value.Indices.Select(index => index + (uint) vertices.Count));
        vertices.AddRange(block_face.Value.Vertices.Select(vertex => new TerrainVertex {
            Position = position_offset + vertex.Position, 
            Color    = Color4.White,
            UV       = BlockAtlas.getUV((int) block.Textures[vertex.TextureID], vertex.UV)
        }));
    }
    
    /*private void constructBlockFace(Block.Block block, BlockModel.FaceInformation? block_face, Vector3 position_offset, int vertex_offset) {
        if (block_face == null)
            return;
        
        indices.AddRange(block_face.Value.Indices.Select(index => index + (uint) vertices.Count + (uint) vertex_offset));
        vertices.AddRange(block_face.Value.Vertices.Select(vertex => new TerrainVertex {
            Position = position_offset + vertex.Position, 
            Color    = Color4.White,
            UV       = BlockAtlas.getUV((int) block.Textures[vertex.TextureID], vertex.UV)
        }));
    }*/
    
    private readonly struct TerrainVertex {
        public Vector3 Position { get; init; }
        public Color4  Color    { get; init; }
        public Vector2 UV       { get; init; }
    }

    private struct ChunkBatchData {
        public int VertexOffset { get; init; }
        public int IndexOffset  { get; init; }
        public int VertexCount  { get; init; }
        public int IndexCount   { get; init; }

        public ChunkBatchData(int vertex_start, int index_start, int vertex_count, int index_count) {
            VertexOffset = vertex_start;
            IndexOffset  = index_start;
            VertexCount  = vertex_count;
            IndexCount   = index_count;
        }
    }
}