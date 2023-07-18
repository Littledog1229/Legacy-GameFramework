using Engine.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Sandbox.Sprite.EditorSprite; 

public class LegacyEditorPhysicsSprite : PhysicsSprite, ILegacyEditorSprite {
    public string EditorTypeName => "PhysicsSprite";
    
    public void drawInspectable() {
        var native_identifier = Identifier;
        var native_pos      = position.toNativeVector2();
        var native_scale    = scale.toNativeVector2();
        var native_rot        = rotation;
        var native_color    = Color.toNativeVector4();

        if (ImGui.InputText("Identifier", ref native_identifier, 100))
            Identifier = native_identifier;
        
        if (ImGui.DragFloat2("Position", ref native_pos))
            Position = native_pos.toVector2();
        if (ImGui.DragFloat2("Scale", ref native_scale))
            Scale = native_scale.toVector2();
        if (ImGui.DragFloat("Rotation", ref native_rot))
            Rotation = native_rot;
        if (ImGui.ColorEdit4("Color", ref native_color))
            Color= native_color.toColor4();
    }

    public void renderPicking(PickingBatch batch, int index) => batch.drawBox((uint) index, position, rotation, 0, scale);
    public void renderOutline(PickingBatch batch, Vector2 outline_size) => batch.drawBox(0, position, rotation, 0, scale + outline_size);

    public LegacyEditorPhysicsSprite(World world, Vector2? initial_scale = null, Vector2? initial_position = null, float initial_rotation = 0.0f, BodyType type = BodyType.Static) 
        : base(world, initial_scale, initial_position, initial_rotation, type) { }
}