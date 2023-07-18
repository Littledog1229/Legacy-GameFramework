using Engine.Physics;
using OpenTK.Mathematics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Sandbox.Sprite; 

public class PhysicsSprite : SpriteObject {
    private readonly BodyType type;
    
    public Body    Body    { get; private set; }
    public Fixture Fixture { get; private set; }
    
    public override Vector2 Position {
        get => position;
        set {
            if (position == value)
                return;

            position = value;
            Body.Position = position.toPhysicsVector2();
        }
    }
    public override Vector2 Scale    {
        get => scale;
        set {
            if (scale == value)
                return;

            scale = value;
            Body.Remove(Fixture);
            Fixture = Body.CreateRectangle(scale.X, scale.Y, 1.0f, tainicom.Aether.Physics2D.Common.Vector2.Zero);
        }
    }
    public override float   Rotation {
        get => rotation;
        set {
            if (Math.Abs(rotation - value) < float.Epsilon)
                return;

            rotation = value;
            Body.Rotation = MathHelper.DegreesToRadians(rotation);
        }
    }

    public PhysicsSprite(World world, Vector2? initial_scale = null, Vector2? initial_position = null, float initial_rotation = 0.0f, BodyType type = BodyType.Static) {
        this.type = type;
        
        initial_scale    ??= Vector2.One;
        initial_position ??= Vector2.Zero;

        position = initial_position.Value;
        scale    = initial_scale.Value;
        rotation = initial_rotation;
        
        Body = world.CreateBody(initial_position.Value.toPhysicsVector2(), MathHelper.DegreesToRadians(initial_rotation), type);
        Fixture = Body.CreateRectangle(initial_scale.Value.X, initial_scale.Value.Y, 1.0f, tainicom.Aether.Physics2D.Common.Vector2.Zero);
    }

    public override void fixedUpdate() {
        if (type == BodyType.Static) 
            return;
        
        position = Body.Position.toVector2();
        rotation = MathHelper.RadiansToDegrees(Body.Rotation);
    }

    public override void destroy() {
        Body.World.Remove(Body);
    }
}