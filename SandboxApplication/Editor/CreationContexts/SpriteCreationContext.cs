using Engine.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using Sandbox.Sprite;

using NativeVector2 = System.Numerics.Vector2;
using NativeVector4 = System.Numerics.Vector4;

namespace Sandbox.Editor.CreationContexts; 

public class SpriteCreationContext : CreationContext {
    protected string        Identifier = "Sprite";
    protected NativeVector2 Position   = NativeVector2.Zero;
    protected NativeVector2 Scale      = NativeVector2.One;
    protected float         Rotation;
    protected NativeVector4 Color      = Color4.White.toNativeVector4();

    public virtual Type SpriteType => typeof(SpriteObject);

    public override void reset() {
        Identifier = "Sprite";
        Position   = NativeVector2.Zero;
        Scale      = NativeVector2.One;
        Rotation   = 0.0f;
        Color      = Color4.White.toNativeVector4();
    }

    public override void renderImgui() {
        ImGui.InputText("Identifier", ref Identifier, 100);
        ImGui.DragFloat2("Position",  ref Position);
        ImGui.DragFloat2("Scale",     ref Scale);
        ImGui.DragFloat("Rotation",   ref Rotation);
        ImGui.ColorEdit4("Color",     ref Color);
    }

    public override object create() {
        return new SpriteObject {
            Identifier = Identifier,
            Position   = Position.toVector2(),
            Scale      = Scale.toVector2(),
            Rotation   = Rotation,
            Color      = Color.toColor4()
        };
    }
}