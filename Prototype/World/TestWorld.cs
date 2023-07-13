using OpenTK.Mathematics;

namespace Prototype.World; 

public class TestWorld {
    public ref Dictionary<uint, string>         MappedIdentifiers => ref mapped_identifiers;
    public ref Dictionary<ChunkPosition, Chunk> LoadedChunks      => ref loaded_chunks;
    public ref Player                           Player            => ref player;

    private Dictionary<uint, string>         mapped_identifiers = new();
    private Dictionary<ChunkPosition, Chunk> loaded_chunks      = new();

    private WorldRenderer renderer;
    private Player        player;
    
    public TestWorld() {
        mapped_identifiers[1] = "dirt";
        mapped_identifiers[2] = "stone";

        player   = new Player(Vector2.Zero);
        renderer = new WorldRenderer(this);

        loadChunksAroundPlayer();
    }

    public void update() {
        var new_chunk = ChunkPosition.worldToChunkPosition(player.Position);
        var old_chunk = ChunkPosition.worldToChunkPosition(player.PreviousPosition);
        
        if (new_chunk != old_chunk)
            loadChunksAroundPlayer();
        
        player.update();
    }

    public void render() => renderer.render();

    private void loadChunksAroundPlayer() {
        Console.WriteLine("Loading Chunks");
        
        var chunk = ChunkPosition.worldToChunkPosition(player.Position);
        var loaded = new Dictionary<ChunkPosition, Chunk>(loaded_chunks);

        for (var x = -player.RenderDistance; x <= player.RenderDistance; x++) {
            for (var y = -player.RenderDistance; y <= player.RenderDistance; y++) {
                tryGenerateChunk(new ChunkPosition(chunk.Position + new Vector2i(x, y)), in loaded);
            }
        }

        foreach (var (pos, _) in loaded)
            loaded_chunks.Remove(pos);
    }

    private void tryGenerateChunk(ChunkPosition pos, in Dictionary<ChunkPosition, Chunk> chunks) {
        if (!chunks.ContainsKey(pos)) {
            var chunk = new Chunk(pos.Position);
            chunk.initialize();
            chunk.generateRandom();

            loaded_chunks.Add(pos, chunk);
            return;
        }

        chunks.Remove(pos);
    }
    
    private void generateChunk(int x, int y) {
        var pos   = new Vector2i(x, y);
        var chunk = new Chunk(pos);
        
        chunk.initialize();
        chunk.generateRandom();

        loaded_chunks[chunk.Position] = chunk;
    }
}