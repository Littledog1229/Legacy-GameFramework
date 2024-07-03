using ApplicationCore.Render.Texture;
using Arch.Core;
using Arch.Core.Extensions;
using Engine.Physics;
using Engine.Utility;
using OpenTK.Mathematics;
using tainicom.Aether.Physics2D.Dynamics;
using World = tainicom.Aether.Physics2D.Dynamics.World;

namespace WaveSurvival.ECS;

// Constructors are ignored when creating an entity without specifying its components, so DO NOT rely on the default constructor
public class TransformComponent {
    private TransformComponent? parent;

    public TransformComponent? Parent {
        get => parent;
        set {
#if DEBUG
            validateParent(this);
#endif
            
            var true_pos   = TruePosition;
            var true_scale = TrueScale;
            var true_rot     = TrueRotation;
            
            parent = value;

            TruePosition = true_pos;
            TrueScale    = true_scale;
            TrueRotation = true_rot;
        }
    }
    public EntityReference     Owner  { get; private set; }
    
    // All values are in local space
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Scale    { get; set; } = Vector2.One;
    public float   Rotation { get; set; } = 0.0f;

    public Vector2 TruePosition {
        get => Parent?.TruePosition + Position ?? Position;
        set => Position = value - Parent?.Position ?? value;
    }
    public Vector2 TrueScale    {
        get => Parent?.TrueScale * Scale ?? Scale;
        set => Scale = value / Parent?.TrueScale ?? value;
    }
    public float   TrueRotation {
        get => Parent?.TrueRotation + Rotation ?? Rotation;
        set => Rotation = value - Parent?.TrueRotation ?? value;
    }

    public TransformComponent(Entity owner, TransformComponent? parent = null) {
        Owner       = owner.Reference();
        this.parent = parent;
    }

#if DEBUG
    private void validateParent(TransformComponent transform) {
        if (parent == transform)
            throw new Exception("Transform references itself as a parent in its parental tree");
        
        parent?.validateParent(transform);
    }
#endif
}

public struct BoxRenderer {
    public Color4 Color { get; set; } = Color4.White;
    public float  Layer { get; set; } = 0.0f;
    
    public BoxRenderer() { }
    public BoxRenderer(Color4 color) => Color = color;
}

public struct SpriteRenderer {
    public Color4    Tint   { get; set; } = Color4.White;
    public Texture2D Sprite { get; set; }
    public float     Layer  { get; set; } = 0.0f;

    public SpriteRenderer(Entity owner, Texture2D sprite, float pixels_per_unit) {
        Sprite = sprite;

        owner.Get<TransformComponent>().Scale = new Vector2(Sprite.Width / pixels_per_unit, Sprite.Height / pixels_per_unit);
    }
}

public sealed class Rigidbody {
    private static Dictionary<Type, Action<Rigidbody>> collider_creators = new();

    public EntityReference Owner { get; }
    public Body            Body  { get; }
    
    public Rigidbody(Entity entity, World physics_world, BodyType type = BodyType.Static) {
        Owner = entity.Reference();
        
        var transform = Owner.Entity.Get<TransformComponent>();
        Body  = physics_world.CreateBody(transform.TruePosition.toPhysicsVector2(), transform.TrueRotation.toRadians(), type);
        
        setupRigidbody(this);
    }

    public static void registerColliderCreator(Type type, Action<Rigidbody> creator) => collider_creators.Add(type, creator);

    private static void setupRigidbody(Rigidbody body) {
        foreach (var (_, value) in collider_creators)
            value.Invoke(body);
    }
}

public struct BoxCollider {
    public Rigidbody Rigidbody { get; set; } = null!;
    public Vector2   Size      { get; set; } = Vector2.One;
    public Vector2   Offset    { get; set; } = Vector2.Zero;
    public Fixture   Fixture   { get; private set; } = null!;
    
    public BoxCollider() { }
    public BoxCollider(Vector2 size) { Size = size; }
    public BoxCollider(Vector2 size, Vector2 offset) { Size = size; Offset = offset; }

    static BoxCollider() {
        Rigidbody.registerColliderCreator(typeof(BoxCollider), tryCreate);
    }

    private static void tryCreate(Rigidbody rigidbody) {
        if (!rigidbody.Owner.Entity.Has<BoxCollider>())
            return;
        
        var collider = rigidbody.Owner.Entity.Get<BoxCollider>();
            
        collider.Fixture = rigidbody.Body.CreateRectangle(collider.Size.X, collider.Size.Y, 1.0f, collider.Offset.toPhysicsVector2());
    }
}