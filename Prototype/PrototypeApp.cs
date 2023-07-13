using Engine.Application;
using Engine.Render;
using Engine.Render.Batch;
using Engine.Render.Camera;
using Prototype.Tile;
using Prototype.World;

namespace Prototype; 

public sealed class PrototypeApp : Application {
    private OrthoCamera camera = null!;
    
    private ShapeBatch batch = null!;

    private TestWorld world;
    
    protected override void initialize() {
        RenderManager.setClearColor(0.5f, 0.5f, 0.5f, 1.0f);

        batch = new ShapeBatch();
        batch.initialize();

        camera = new OrthoCamera();
        camera.initialize();
        
        TileManager.initialize();
        world = new TestWorld();
    }

    protected override void onUpdate() {
        world.update();
    }

    protected override void onRender() {
        world.render();
    }
}