using System.Reflection;
using Engine.Application;
using Engine.Render;
using Engine.Render.Batch;
using Engine.Render.Camera;
using Engine.Render.Texture;
using OpenTK.Mathematics;

namespace Sandbox; 

public readonly struct TestVertex {
    public Vector2 Position     { get; init; }
    public Color4  Color        { get; init; }
    public Vector2 UV           { get; init; }
    public int     TextureCoord { get; init; }

    public TestVertex(Vector2 position, Color4 color, Vector2 uv, int coord) {
        Position     = position;
        Color        = color;
        UV           = uv;
        TextureCoord = coord;
    }
}

public sealed class SandboxApp : Application {
    private OrthoCamera camera = null!;
    
    private ShapeBatch batch  = new();
    private Matrix4    matrix = Matrix4.Identity;

    private TextureAtlas            atlas   = null!;
    private TextureAtlas            overlay = null!;
    private TextureAtlas.SubTexture grass;
    private TextureAtlas.SubTexture dirt;
    private TextureAtlas.SubTexture stone;
    
    protected override void initialize() {
        RenderManager.setClearColor(0.25f, 0.25f, 0.25f, 1.0f);

        batch.initialize();
        
        camera = new OrthoCamera();
        camera.initialize();

        matrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

        atlas   = new TextureAtlas("Resource.Texture.Tiles.png");
        overlay = new TextureAtlas("Resource.Texture.Overlays.png");

        grass = overlay.create("grass", new Vector2i(1, 1), new Vector2i(8, 8));
        dirt  = atlas.create  ("dirt",  new Vector2i(1, 1),  new Vector2i(8, 8));
        stone = atlas.create  ("stone", new Vector2i(11, 1), new Vector2i(8, 8));
    }

    protected override void onRender() {
        batch.begin(camera);
        
        batch.drawBox(stone, new Vector2(0.0f, 0.0f), Color4.White);
        batch.drawBox(grass, new Vector2(0.0f, 0.0f), Color4.Beige);
        batch.drawBox(stone, new Vector2(1.0f, 0.0f), Color4.White);
        batch.drawBox(grass, new Vector2(1.0f, 0.0f), Color4.Beige);
        batch.drawBox(stone, new Vector2(-1.0f, 0.0f), Color4.White);
        batch.drawBox(grass, new Vector2(-1.0f, 0.0f), Color4.Beige);
        batch.drawBox(stone, new Vector2(0.0f, -1.0f), Color4.White);
        
        batch.end();
    }

    protected override void onResize() {
        camera.resize();
    }
}