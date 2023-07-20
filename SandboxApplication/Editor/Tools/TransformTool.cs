using ApplicationCore.Application;
using Engine.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using NativeVector2 = System.Numerics.Vector2;

namespace Sandbox.Editor.Tools; 

public sealed class TransformTool : EditorTool {
    public override string Identifier => "Transform";

    public TransformTool(EditorScene owner) : base(owner) { }

    private bool transform_x  = false;
    private bool transform_y  = false;
    private bool transform_xy = false;
    private bool snap_grid    = false;

    private Vector2 offset = Vector2.Zero;

    public override void updateWidget() {
        if (Sprite == null)
            return;
        
        var camera = Owner.Camera;
        var delta = InputManager.MouseDelta;

        var mouse_pos   = Owner.ViewportMousePosition;
        var sprite_pos  = Sprite!.Sprite.Position;
        var screen_pos  = Owner.ViewportOffset + camera.worldToScreenSpace(sprite_pos).toNativeVector2();
        var world_delta = camera.screenScaleToWorldScale(delta);

        var box_pos = screen_pos + new NativeVector2(20.0f, -20.0f);
        var left_press = InputManager.mousePress(MouseButton.Left);
        
        // Handle Logic
        // XY
        ImGui.SetCursorScreenPos(box_pos - new NativeVector2(20.0f, 0.0f));
        ImGui.InvisibleButton("TransformTool_XYBox", new NativeVector2(20.0f, 20.0f));

        if (ImGui.IsItemHovered() && left_press) {
            transform_x       = false;
            transform_y       = false;
            transform_xy      = true;
            offset            = sprite_pos - camera.screenToWorldSpace(mouse_pos);
            Owner.CanUpdateSelected = false;
        }

        // X
        ImGui.SetCursorScreenPos(screen_pos - new NativeVector2(0, 1.5f));
        ImGui.InvisibleButton("TransformTool_XBox", new NativeVector2(40.0f, 3.0f));
        
        if (ImGui.IsItemHovered() && left_press) {
            transform_x       = true;
            transform_y       = false;
            transform_xy      = false;
            offset            = sprite_pos - camera.screenToWorldSpace(mouse_pos);
            Owner.CanUpdateSelected = false;
        }
        
        // Y
        ImGui.SetCursorScreenPos(screen_pos - new NativeVector2(1.0f, 40.0f));
        ImGui.InvisibleButton("TransformTool_YBox", new NativeVector2(3.0f, 40.0f));
        
        if (ImGui.IsItemHovered() && left_press) {
            transform_x       = false;
            transform_y       = true;
            transform_xy      = false;
            offset            = sprite_pos - camera.screenToWorldSpace(mouse_pos);
            Owner.CanUpdateSelected = false;
        }
        
        if (InputManager.mouseUp(MouseButton.Left)) {
            transform_x  = false;
            transform_y  = false;
            transform_xy = false;

            Owner.CanUpdateSelected = true;
        }

        snap_grid = InputManager.keyDown(Keys.LeftControl);

        var offset_position = camera.screenToWorldSpace(Owner.ViewportMousePosition) + offset;

        if (snap_grid)
            offset_position = new Vector2(MathF.Floor(offset_position.X), MathF.Floor(offset_position.Y));
        
        if (transform_x  && world_delta != Vector2.Zero)
            Sprite!.Sprite.Position = new Vector2(offset_position.X, sprite_pos.Y);
        
        if (transform_y  && world_delta != Vector2.Zero)
            Sprite!.Sprite.Position = new Vector2(sprite_pos.X, offset_position.Y);
        
        if (transform_xy && world_delta != Vector2.Zero)
            Sprite!.Sprite.Position = offset_position;
        
        screen_pos = Owner.ViewportOffset + camera.worldToScreenSpace(Sprite!.Sprite.Position).toNativeVector2();
        box_pos    = screen_pos + new NativeVector2(20.0f, -20.0f);
        
        var draw_list = ImGui.GetWindowDrawList();
        // Draw Transform Box
        draw_list.AddRectFilled(screen_pos, box_pos, 0x77ff0000);
        draw_list.AddRect(screen_pos, box_pos, 0xffff0000);
        
        // Draw Axis
        draw_list.AddLine(screen_pos - new NativeVector2(0.0f, 0.0f), screen_pos - new NativeVector2(00.0f, 40.0f), 0xff00ff00, 3.0f); // Y
        draw_list.AddLine(screen_pos - new NativeVector2(1.5f, 0.0f), screen_pos + new NativeVector2(40.0f, 00.0f), 0xff0000ff, 3.0f); // X
    }

    protected override void updateSelectedSprite() {
        transform_x  = false;
        transform_y  = false;
        transform_xy = false;
    }
}