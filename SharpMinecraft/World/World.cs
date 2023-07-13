using Engine.Render.Camera;
using OpenTK.Mathematics;

namespace SharpMinecraft.World; 

public sealed class World {
    private Player player;
    private Camera camera;
    //private TestChunkRenderManager manager;
    private BatchedChunkRenderer batched_renderer;
    
    private Dictionary<ChunkPosition, Chunk> loaded_chunks = new();

    private int render_distance = 5;
    
    public Action<Chunk>? OnChunkLoad   { get; set; }
    public Action<Chunk>? OnChunkUnload { get; set; }

    public FastNoiseLite Noise { get; } = new();

    public World() {
        player           = new Player(this);
        camera           = player.Camera;
        batched_renderer = new(this);
        //manager = new TestChunkRenderManager(this);
        Noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        loadChunksAroundPlayer(true);
    }

    public void update() {
        player.update();
        
        //loadChunksAroundPlayer();
        
        //Console.WriteLine(GC.GetTotalMemory(false) / 1000000);
    }

    public void render() {
        //test_chunk_renderer.render(camera);
        //manager.render(camera);
        batched_renderer.render(camera);
    }

    public void destroy() {
        batched_renderer.destroy();
    }

    public uint getBlock(Vector3i position) {
        if (position.Y >= Chunk.CHUNK_SIZE_Y)
            return 0;
        
        var (chunk, local) = ChunkPosition.toChunkLocalSpace(position);

        return !loaded_chunks.ContainsKey(chunk) ? 0 : loaded_chunks[chunk][local.X, local.Y, local.Z];
    }
    public uint getBlockRelative(Vector3i position, ChunkPosition chunk_position) {
        if (position.Y >= Chunk.CHUNK_SIZE_Y)
            return 0;
        
        var (local_chunk, pos) = ChunkPosition.toChunkLocalSpace(position);

        var true_chunk = chunk_position + local_chunk;
        
        return !loaded_chunks.ContainsKey(true_chunk) ? 0 : loaded_chunks[true_chunk][pos.X, pos.Y, pos.Z];
    }

    public bool chunkExists(ChunkPosition position)     => loaded_chunks.ContainsKey(position);
    public bool chunkExists(ref ChunkPosition position) => loaded_chunks.ContainsKey(position);
    
    private void loadChunksAroundPlayer(bool ignore_pos = false) {
        var player_chunk_pos = ChunkPosition.toChunkPosition(player.Position);
        var prev_chunk_pos   = ChunkPosition.toChunkPosition(player.LastPosition);

        if (!ignore_pos && player_chunk_pos == prev_chunk_pos)
            return;

        var unload = new List<ChunkPosition>(loaded_chunks.Keys);

        for (var x = -render_distance; x <= render_distance; x++) {
            for (var z = -render_distance; z <= render_distance; z++) {
                var chunk_pos = new ChunkPosition(x, z) + player_chunk_pos;

                if (unload.Contains(chunk_pos)) {
                    unload.Remove(chunk_pos);
                    continue; // Chunk is already loaded
                }

                var chunk = new Chunk(this, chunk_pos);
                //chunk.fillSolid();
                chunk.generateTerrain();
                
                loaded_chunks.Add(chunk_pos, chunk);
                OnChunkLoad?.Invoke(chunk);
            }
        }

        foreach (var pos in unload) {
            var chunk = loaded_chunks[pos];
            loaded_chunks.Remove(pos);
            
            OnChunkUnload?.Invoke(chunk);
        }
    }

    public bool tryGetChunk(ChunkPosition pos, out Chunk? chunk) => loaded_chunks.TryGetValue(pos, out chunk);
}