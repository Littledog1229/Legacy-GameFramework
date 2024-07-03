using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Camera;
using ApplicationCore.Render.Pipeline;
using ApplicationCore.Render.Texture;
using Arch.Core;
using Arch.Core.Extensions;
using Engine.Utility;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using tainicom.Aether.Physics2D.Dynamics;
using WaveSurvival.ECS;
using WaveSurvival.ECS.Query;

using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;

namespace WaveSurvival; 

public sealed class SingleplayerScene : EntityScene {
    public OrthoCamera Camera = null!;
    public ShapeBatch  Batch  = null!;

    protected override void   registerRender() => RenderManager.getActiveRenderPipeline<DefaultRenderPipeline>().OnRender += render;
    protected override void deregisterRender() => RenderManager.getActiveRenderPipeline<DefaultRenderPipeline>().OnRender -= render;

    private readonly Texture2D player_sprite = new("Resource.Texture.Player.png");
    private readonly Texture2D weapon_sprite = new("Resource.Texture.GunButSuck.png");

    private Entity player;
    private Entity weapon;
    
    protected override void registerQueries() {
        FixedUpdateQueries.Add(new RigidbodyQuery());
        
        RenderQueries.Add(new BoxRendererQuery());
        RenderQueries.Add(new SpriteRendererQuery());
    }

    public override void start() {
        base.start();

        RenderManager.OnResize += resize;

        Camera = new OrthoCamera();
        Camera.initialize();

        Batch = new ShapeBatch();
        Batch.initialize();

        {
            var entity = createEntity();

            var transform = entity.Get<TransformComponent>();
            transform.Position = new Vector2(0.0f, -1.5f);
            transform.Scale    = new Vector2(5.0f, 0.25f);
            
            entity.Add(new BoxRenderer());
            entity.Add(new BoxCollider(new Vector2(5.0f, 0.25f)));
            entity.Add(new Rigidbody(entity, PhysicsWorld));
        }

        {
            player = createEntity();
            player.Add(new SpriteRenderer(player, player_sprite, 16.0f));
            player.Get<TransformComponent>().Position = new Vector2(0.0f, 2.0f);
            
            player.Add(new BoxCollider());
            player.Add(new Rigidbody(player, PhysicsWorld, BodyType.Dynamic));
        }
        
        weapon = createEntity(player.Get<TransformComponent>());
        var weapon_renderer = new SpriteRenderer(weapon, weapon_sprite, 16.0f) { Layer = 1.0f };
        weapon.Add(weapon_renderer);
    }

    public override void destroy() {
        base.destroy();
        
        RenderManager.OnResize -= resize;

        Batch.destroy();
        player_sprite.destroy();
        weapon_sprite.destroy();
    }

    public Entity createEntity(TransformComponent? parent = null) {
        var entity = World.Create();
        entity.Add(new TransformComponent(entity, parent));

        return entity;
    }

    private float speed = 2.0f;
    
    public override void update() {
        var move_x = (InputManager.keyDown(Keys.A) ? 0.0f : 1.0f) - (InputManager.keyDown(Keys.D) ? 0.0f : 1.0f);
        var move_y = (InputManager.keyDown(Keys.S) ? 0.0f : 1.0f) - (InputManager.keyDown(Keys.W) ? 0.0f : 1.0f);

        var player_rigidbody = player.Get<Rigidbody>();
        var weapon_transform = weapon.Get<TransformComponent>();
        
        var mouse_world = Camera.screenToWorldSpace(InputManager.MousePosition);
        var aim_vector = (mouse_world - weapon_transform.TruePosition).Normalized();

        var velocity_x = move_x * speed;
        var velocity_y = player_rigidbody.Body.LinearVelocity.Y;

        if (InputManager.keyPress(Keys.Space))
            velocity_y = 5.0f;
        
        player_rigidbody.Body.LinearVelocity = new tainicom.Aether.Physics2D.Common.Vector2(velocity_x, velocity_y);
        weapon_transform.TrueRotation = MathF.Atan2(aim_vector.Y, aim_vector.X).toDegrees();
    }

    private void resize(int width, int height) {
        Camera.resize(width, height);
    }
}