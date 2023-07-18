using System.Numerics;
using ApplicationCore.Render.Batch;
using Engine.Physics;
using ImGuiNET;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Sandbox.Sprite.EditorSprite; 

public class LegacyEditorSprite : SpriteObject, ILegacyEditorSprite {
    public string EditorTypeName => "Sprite";

    public virtual void drawInspectable() {
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

    public virtual void renderPicking(PickingBatch batch, int index) => batch.drawBox((uint) index, position, rotation, 0, scale);
    public virtual void renderOutline(PickingBatch batch, Vector2 outline_size) => batch.drawBox(0, position, rotation, 0, scale + outline_size);
}