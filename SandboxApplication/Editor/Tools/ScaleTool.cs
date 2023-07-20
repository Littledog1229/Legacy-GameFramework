using ApplicationCore.Application;
using Engine.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using NativeVector2 = System.Numerics.Vector2;

namespace Sandbox.Editor.Tools; 

public sealed class ScaleTool : EditorTool {
    public override string Identifier => "Scale";

    public ScaleTool(EditorScene owner) : base(owner) { }

    private bool scale_x;
    private bool scale_y;
    private bool snap_grid;
    private bool snap_small;

    private Vector2 offset      = Vector2.Zero;
    private Vector2 start_scale = Vector2.One;

    public override void updateWidget() {
        if (Sprite == null)
            return;
        
        var camera = Owner.Camera;
        var delta = InputManager.MouseDelta;

        var mouse_pos   = Owner.ViewportMousePosition;
        var sprite_pos  = Sprite!.Sprite.Position;
        var screen_pos  = Owner.ViewportOffset + camera.worldToScreenSpace(sprite_pos).toNativeVector2();
        var world_delta = camera.screenScaleToWorldScale(delta);

        var left_press = InputManager.mousePress(MouseButton.Left);
        
        // Handle Logic
        // X
        ImGui.SetCursorScreenPos(screen_pos - new NativeVector2(0, 1.5f));
        ImGui.InvisibleButton("ScaleTool_XBox", new NativeVector2(40.0f, 3.0f));
        
        if (ImGui.IsItemHovered() && left_press) {
            scale_x     = true;
            scale_y     = false;
            offset      = sprite_pos - camera.screenToWorldSpace(mouse_pos);
            start_scale = Sprite!.Sprite.Scale;
            Owner.CanUpdateSelected = false;
        }
        
        // Y
        ImGui.SetCursorScreenPos(screen_pos - new NativeVector2(1.0f, 40.0f));
        ImGui.InvisibleButton("ScaleTool_YBox", new NativeVector2(3.0f, 40.0f));
        
        if (ImGui.IsItemHovered() && left_press) {
            scale_x = false;
            scale_y = true;
            offset  = sprite_pos - camera.screenToWorldSpace(mouse_pos);
            start_scale = Sprite!.Sprite.Scale;
            Owner.CanUpdateSelected = false;
        }
        
        if (InputManager.mouseUp(MouseButton.Left)) {
            scale_x = false;
            scale_y = false;

            Owner.CanUpdateSelected = true;
        }

        snap_grid  = InputManager.keyDown(Keys.LeftControl);
        snap_small = InputManager.keyDown(Keys.LeftShift);

        var offset_position = camera.screenToWorldSpace(Owner.ViewportMousePosition) + offset;

        if (snap_grid && !snap_small)
            offset_position = new Vector2(MathF.Floor(offset_position.X), MathF.Floor(offset_position.Y));

        if (snap_grid && snap_small)
            offset_position = new Vector2(MathF.Floor(offset_position.X / 0.25f) * 0.25f, MathF.Floor(offset_position.Y / 0.25f) * 0.25f);
        
        
        
        if (scale_x && world_delta != Vector2.Zero)
            Sprite!.Sprite.Scale = start_scale + new Vector2(offset_position.X, 0.0f);
        
        if (scale_y && world_delta != Vector2.Zero)
            Sprite!.Sprite.Scale = start_scale + new Vector2(0.0f, offset_position.Y);
        
        screen_pos = Owner.ViewportOffset + camera.worldToScreenSpace(Sprite!.Sprite.Position).toNativeVector2();
        
        var draw_list = ImGui.GetWindowDrawList();
        // Draw Axis
        draw_list.AddLine(screen_pos - new NativeVector2(0.0f, 0.0f), screen_pos - new NativeVector2(00.0f, 40.0f), 0xff00ff00, 3.0f); // Y
        draw_list.AddLine(screen_pos - new NativeVector2(1.5f, 0.0f), screen_pos + new NativeVector2(40.0f, 00.0f), 0xff0000ff, 3.0f); // X
    }

    protected override void updateSelectedSprite() {
        scale_x = false;
        scale_y = false;
    }
}