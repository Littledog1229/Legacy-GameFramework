using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Camera;
using ApplicationCore.Render.Texture;
using Engine.Physics;
using Engine.Scenes;
using OpenTK.Mathematics;
using Sandbox.Sprite;
using tainicom.Aether.Physics2D.Dynamics;

namespace Sandbox; 

public sealed class TestScene : Scene {
    private readonly World       physics_world = PhysicsManager.createWorld();
    private readonly Texture2D   player_texture = new("Resource.Texture.temp_player.png");
    private readonly ShapeBatch  batch  = new();
    private readonly OrthoCamera camera = new();

    private readonly List<SpriteObject> sprites = new();
    
    public override void start() {
        Console.WriteLine("Started TestScene!");

        camera.ViewSizeX = 10.0f;

        batch.initialize();
        camera.initialize();

        var player = new PhysicsSprite(physics_world, type: BodyType.Dynamic) {
            Texture = player_texture
        };

        var test = new PhysicsSprite(physics_world, new Vector2(3.0f, 0.25f), new Vector2(-1f, -2.0f), initial_rotation: -20.0f) {
            Color = Color4.Orange
        };

        var floor = new PhysicsSprite(physics_world, new Vector2(10.0f, 0.25f), new Vector2(0.0f, -5.0f));

        sprites.Add(player);
        sprites.Add(test);
        sprites.Add(floor);
    }

    public override void destroy() {
        Console.WriteLine("Destroyed TestScene!");
    }

    public override void render() {
        batch.begin(camera);
        
        foreach (var sprite in sprites)
            sprite.render(batch);
        
        batch.end();
    }

    public override void update() {
        foreach (var sprite in sprites)
            sprite.update();
    }
    public override void fixedUpdate() {
        foreach (var sprite in sprites)
            sprite.fixedUpdate();
    }
    public override void lateUpdate() {
        foreach (var sprite in sprites)
            sprite.fixedUpdate();
    }
}