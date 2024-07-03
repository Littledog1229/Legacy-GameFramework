using ApplicationCore.Render;
using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Camera;
using ApplicationCore.Render.Pipeline;
using Arch.Core;
using Arch.Core.Extensions;
using Engine;
using Engine.Physics;
using Engine.Scenes;
using Engine.Utility;
using OpenTK.Mathematics;
using tainicom.Aether.Physics2D.Dynamics;

using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;
using ECSWorld     = Arch.Core.World;

namespace Sandbox;

public struct TransformComponent   {
    public Vector2 Position { get; set; }
    public Vector2 Scale    { get; set; }
    public float   Rotation { get; set; }
    public float   Layer    { get; set; }

    public TransformComponent() {
        Position = Vector2.Zero;
        Scale    = Vector2.One;
        Rotation = 0.0f;
        Layer    = 0.0f;
    }
}
public struct BoxRendererComponent {
    public Color4 Color { get; set; }

    public BoxRendererComponent()             { Color = Color4.White; }
    public BoxRendererComponent(Color4 color) { Color = color; }
}
public struct BoxColliderComponent {
    public Vector2 Size { get; }

    public BoxColliderComponent(Vector2 size) { Size = size; }
}

public class RigidbodyComponent {
    public Body Body { get; }

    public RigidbodyComponent(Entity owner, PhysicsWorld physics, BodyType body_type = BodyType.Static) {
        var transform = owner.Get<TransformComponent>();

        Body = physics.CreateBody(transform.Position.toPhysicsVector2(), transform.Rotation.toRadians(), body_type);

        if (!owner.Has<BoxColliderComponent>()) 
            return;
        
        var box = owner.Get<BoxColliderComponent>();
        Body.CreateRectangle(box.Size.X, box.Size.Y, 1.0f, Vector2.One.toPhysicsVector2() / 2.0f);
    }
}

public sealed class EcsScene : Scene {
    private ECSWorld     ecs_world     = null!;
    private PhysicsWorld physics_world = null!;

    private OrthoCamera camera = null!;
    private ShapeBatch  batch  = null!;

    private readonly QueryDescription box_description       = new QueryDescription().WithAll<TransformComponent, BoxRendererComponent, BoxColliderComponent>();
    private readonly QueryDescription rigidbody_description = new QueryDescription().WithAll<TransformComponent, RigidbodyComponent>();

    public override void start() {
        ecs_world     = ECSWorld.Create();
        physics_world = new PhysicsWorld();
        
        var floor_entity = ecs_world.Create(new TransformComponent { Position = new Vector2(0.0f, -2.0f), Scale = new Vector2(5.0f, 0.25f)}, new BoxRendererComponent(Color4.White), new BoxColliderComponent(new Vector2(5.0f, 0.25f)));
        floor_entity.Add(new RigidbodyComponent(floor_entity, physics_world));

        var box_entity = ecs_world.Create(new TransformComponent(), new BoxRendererComponent(Color4.SaddleBrown), new BoxColliderComponent(Vector2.One));
        box_entity.Add(new RigidbodyComponent(box_entity, physics_world, BodyType.Dynamic));

        var box2_entity = ecs_world.Create(new TransformComponent(), new BoxRendererComponent(Color4.SaddleBrown), new BoxColliderComponent(Vector2.One));
        box2_entity.Add(new RigidbodyComponent(box2_entity, physics_world, BodyType.Dynamic));
        
        var box3_entity = ecs_world.Create(new TransformComponent(), new BoxRendererComponent(Color4.SaddleBrown), new BoxColliderComponent(Vector2.One));
        box3_entity.Add(new RigidbodyComponent(box3_entity, physics_world, BodyType.Dynamic));
        
        batch  = new ShapeBatch();
        camera = new OrthoCamera();
        
        batch.initialize();
        camera.initialize();

        RenderManager.getActiveRenderPipeline<DefaultRenderPipeline>().OnRender += render;
    }

    public override void fixedUpdate() {
        physics_world.Step(Time.FixedDeltaTime);

        ecs_world.Query(rigidbody_description, (ref TransformComponent transform, ref RigidbodyComponent rigidbody) => {
            transform.Position = rigidbody.Body.Position.toVector2();
            transform.Rotation = rigidbody.Body.Rotation.toDegrees();
        });
    }

    private void render(RenderPipeline pipeline) {
        batch.begin(camera);

        ecs_world.Query(box_description, (ref TransformComponent transform, ref BoxRendererComponent renderer) => {
            batch.drawBox(transform.Position, renderer.Color, transform.Rotation, transform.Layer, transform.Scale);
        });
        
        batch.end();
    }

    public override void destroy() {
        batch.destroy();
        ecs_world.Dispose();
        
        RenderManager.getActiveRenderPipeline<DefaultRenderPipeline>().OnRender -= render;
    }
}