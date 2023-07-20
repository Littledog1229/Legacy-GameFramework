using ImGuiNET;
using Sandbox.Editor.CreationContexts;
using Sandbox.Sprite;
using tainicom.Aether.Physics2D.Dynamics;

namespace Sandbox.Editor.Inspectors; 

public class PhysicsSpriteInspector : CustomInspector {
    public override void drawInspectable(SpriteObject sprite) {
        base.drawInspectable(sprite);

        var physics_sprite   = (sprite as PhysicsSprite)!;
        var native_body_type = (int) physics_sprite.Body.BodyType;

        if (ImGui.Combo("Body Type", ref native_body_type, PhysicsSpriteCreationContext.MAPPED_BODY_TYPES, 3))
            physics_sprite.Body.BodyType = (BodyType) native_body_type;
    }
}