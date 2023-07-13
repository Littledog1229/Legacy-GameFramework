using Engine.Render.Batch;
using Prototype.Tile;

namespace Prototype.World; 

public sealed class WorldRenderer {
    private readonly TestWorld  world;
    private readonly ShapeBatch batch;

    public WorldRenderer(TestWorld world) {
        this.world = world;
        
        batch = new ShapeBatch(40000, 60000);
        batch.initialize();
    }

    public void render() {
        batch.begin(world.Player.Camera);

        var loaded_chunks = world.LoadedChunks;
        var mapped_indexes = world.MappedIdentifiers;
        
        foreach (var (_, chunk) in loaded_chunks) {
            for (var x = 0; x < Chunk.CHUNK_SIZE_X; x++) {
                for (var y = 0; y < Chunk.CHUNK_SIZE_Y; y++) {
                    var tile_index = chunk[x, y].TileIndex;
                    if (tile_index == 0)
                        continue;
                        
                    
                    TileManager.getRegisteredTile(mapped_indexes[tile_index]).render(chunk[x, y], batch);
                }
            }
        }
        
        world.Player.render(batch);
        
        batch.end();
    }
}