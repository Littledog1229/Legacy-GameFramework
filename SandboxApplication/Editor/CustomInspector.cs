using Engine.Physics;
using ImGuiNET;
using Sandbox.Sprite;

namespace Sandbox.Editor; 

public abstract class CustomInspector {
    public virtual void drawInspectable(SpriteObject sprite) {
        var native_identifier = sprite.Identifier;
        var native_pos = sprite.Position.toNativeVector2();
        var native_scale = sprite.Scale.toNativeVector2();
        var native_rot = sprite.Rotation;
        var native_color = sprite.Color.toNativeVector4();

        if (ImGui.InputText("Identifier", ref native_identifier, 100))
            sprite.Identifier = native_identifier;
        if (ImGui.DragFloat2("Position", ref native_pos))
            sprite.Position = native_pos.toVector2();
        if (ImGui.DragFloat2("Scale", ref native_scale))
            sprite.Scale = native_scale.toVector2();
        if (ImGui.DragFloat("Rotation", ref native_rot))
            sprite.Rotation = native_rot;
        if (ImGui.ColorEdit4("Color", ref native_color))
            sprite.Color= native_color.toColor4();
    }
}