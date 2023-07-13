using Engine.Render;
using Engine.Render.Camera;

namespace SharpMinecraft.World; 

public sealed class TestChunkRenderManager {
    public static Shader TerrainShader { get; private set; }
    
    private World world;
    private readonly Dictionary<Chunk, ChunkRenderer> renderers = new();
    private readonly List<Chunk> renderers_to_create = new();

    public TestChunkRenderManager(World world) {
        this.world = world;
        world.OnChunkLoad   += addChunk;
        world.OnChunkUnload += removeChunk;
        
        TerrainShader = RenderManager.getOrCreateShader("terrain", "Resource.Shader.Terrain.glsl");
    }

    public void render(Camera camera) {
        if (renderers_to_create.Count > 0) {
            var chunk = renderers_to_create[0];
            renderers_to_create.RemoveAt(0);
            
            var renderer = new ChunkRenderer(chunk);
            renderers.Add(chunk, renderer);
        }

        foreach (var renderer in renderers.Values) {
            renderer.render(camera);
        }
    }

    private void addChunk(Chunk chunk) {
        renderers_to_create.Add(chunk);
        //var renderer = new ChunkRenderer(chunk);
        //renderers.Add(chunk, renderer);
    }
    
    private void removeChunk(Chunk chunk) {
        if (renderers_to_create.Remove(chunk))
            return;
        
        renderers[chunk].destroy();
        renderers.Remove(chunk);
    }
}