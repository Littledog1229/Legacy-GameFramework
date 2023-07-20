using ApplicationCore.Application;
using Engine.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using Engine.Utility;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sandbox.Editor.Tools; 

public sealed class RotationTool : EditorTool {
    public RotationTool(EditorScene owner) : base(owner) { }

    public override string Identifier => "Rotation";

    private bool  dragging;
    private bool  snap_rotation;
    private float start_rotation;
    private float rotation_offset;
    
    // TODO: Show rotation change
    
    public override void updateWidget() {
        if (Sprite == null)
            return;
        
        var camera = Owner.Camera;
        
        var draw_list = ImGui.GetWindowDrawList();
        var sprite_pos = Sprite!.Sprite.Position;
        var screen_pos = Owner.ViewportOffset + camera.worldToScreenSpace(sprite_pos).toNativeVector2();

        ImGui.SetCursorScreenPos(screen_pos - new System.Numerics.Vector2(55.0f, 55.0f));
        ImGui.InvisibleButton("RotationTool", new System.Numerics.Vector2(110, 110));

        var mouse_offset = InputManager.MousePosition - screen_pos.toVector2();
        var mouse_offset_normalized = mouse_offset.Normalized();
        
        if (ImGui.IsItemHovered() && InputManager.mousePress(MouseButton.Left)) {
            var mouse_vector_length = mouse_offset.Length;

            if (mouse_vector_length is >= 45.0f and <= 55.0f) {
                dragging                = true;
                start_rotation          = Sprite.Sprite.Rotation;
                Owner.CanUpdateSelected = false;

                rotation_offset =
                    (MathHelper.RadiansToDegrees(MathF.Atan2(-mouse_offset_normalized.Y, mouse_offset_normalized.X)) -
                     Sprite.Sprite.Rotation);
            }
        }

        if (dragging && InputManager.mouseUp(MouseButton.Left)) {
            dragging = false;
            Owner.CanUpdateSelected = true;
        }

        snap_rotation = InputManager.keyDown(Keys.LeftControl);

        if (dragging && InputManager.MouseDelta != Vector2.Zero) {
            var rotation = MathHelper.RadiansToDegrees(MathF.Atan2(-mouse_offset_normalized.Y, mouse_offset_normalized.X)) - rotation_offset;

            if (snap_rotation)
                rotation = MathF.Floor(rotation / 15.0f) * 15.0f;
            
            Sprite.Sprite.Rotation = rotation % 360.0f;
        }
        
        var rot_center = new Vector2(50.0f, 0.0f).rotateAround(Vector2.Zero, Sprite.Sprite.Rotation + 180.0f);

        draw_list.AddCircle(screen_pos, 50.0f, 0xff66ff66, 32, 3.0f);
        draw_list.AddCircleFilled(rot_center.toNativeVector2() + screen_pos, 5.0f, 0xff00ff00);
    }

    protected override void updateSelectedSprite() {
        dragging       = false;
        start_rotation = Sprite == null ? 0.0f : Sprite!.Sprite.Rotation;
    }
}