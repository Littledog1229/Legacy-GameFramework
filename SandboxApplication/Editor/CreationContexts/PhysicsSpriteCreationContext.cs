using Engine.Physics;
using ImGuiNET;
using Sandbox.Sprite;
using tainicom.Aether.Physics2D.Dynamics;

namespace Sandbox.Editor.CreationContexts; 

public class PhysicsSpriteCreationContext : SpriteCreationContext {
    private World physics_world;

    public static readonly string[] MAPPED_BODY_TYPES = {
        nameof(BodyType.Static),
        nameof(BodyType.Kinematic),
        nameof(BodyType.Dynamic)
    };

    private int int_body_type = 0;
    
    protected BodyType BodyType = BodyType.Static;

    public PhysicsSpriteCreationContext(World world) { physics_world = world; }
    
    public override Type SpriteType => typeof(PhysicsSprite);
    public override object create() {
        return new PhysicsSprite(physics_world, Scale.toVector2(), Position.toVector2(), Rotation, BodyType) {
            Identifier = Identifier,
            Color = Color.toColor4()
        };
    }

    public override void renderImgui() {
        base.renderImgui();

        if (ImGui.Combo("Body Type", ref int_body_type, MAPPED_BODY_TYPES, MAPPED_BODY_TYPES.Length))
            BodyType = (BodyType) int_body_type;
    }

    public override void reset() {
        base.reset();

        int_body_type = 0;
        BodyType      = BodyType.Static;
    }
}