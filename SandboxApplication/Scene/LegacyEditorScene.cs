using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Camera;
using ApplicationCore.Render.Texture;
using Engine;
using Engine.Physics;
using Engine.Scenes;
using Engine.Utility;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sandbox.Sprite;
using Sandbox.Sprite.EditorSprite;
using tainicom.Aether.Physics2D.Dynamics;

using NativeVector2  = System.Numerics.Vector2;
using PhysicsVector2 = tainicom.Aether.Physics2D.Common.Vector2;

namespace Sandbox; 

public sealed class LegacyEditorScene : Scene {
    private static readonly NativeVector2 VIEWPORT_UV_0 = new(0.0f, 1.0f);
    private static readonly NativeVector2 VIEWPORT_UV_1 = new(1.0f, 0.0f);

    private readonly Shader          outline_shader = RenderManager.getOrCreateShader("Outline", "Resource.Shader.Outline.glsl");
    private readonly World           physics_world  = new();
    private readonly Texture2D       player_texture = new("Resource.Texture.temp_player.png");
    private readonly ShapeBatch      batch          = new();
    private readonly PickingBatch    picking_batch  = new();
    private readonly OrthoCamera     camera         = new();
    
    private readonly List<SpriteObject> sprites = new();

    private Framebuffer         framebuffer    = null!;
    private Framebuffer         picking_buffer = null!;
    private LegacyEditorPhysicsSprite player         = null!;

    private SpriteObject? selected;

    private ImGuiController imgui_controller = new(RenderManager.WindowWidth, RenderManager.WindowHeight);

    private IntPtr        viewport_texture_ptr;
    private NativeVector2 viewport_size;
    private NativeVector2 viewport_offset;
    private NativeVector2 viewport_content_offset;
    private NativeVector2 old_viewport_size;
    private NativeVector2 old_viewport_offset;
    private bool          can_focus_viewport = true;
    private bool          viewport_focused;

    private Vector2 outline_size   = new(0.075f, 0.075f);
    private Color4  outline_color  = Color4.OrangeRed;
    private bool    render_outline = false;
    
    private const float CAMERA_MOVE_SPEED = 1.0f;
    
    private Vector2 ViewportMousePosition => InputManager.MousePosition - new Vector2(viewport_offset.X, viewport_offset.Y);
    
    public override void start() {
        Console.WriteLine("Started TestScene!");

        ImGui.LoadIniSettingsFromDisk("test_scene");
        
        camera.ViewSizeX = 10.0f;

        batch.initialize();
        picking_batch.initialize();
        camera.initialize();

        {
            var info = new Framebuffer.FramebufferInfo {
                HasDepth = true
            };
            
            info.addColorInfo(new Framebuffer.FramebufferInfo.AttachmentInfo {
                InternalPixelFormat = PixelInternalFormat.Rgb32ui,
                PixelFormat = PixelFormat.RgbInteger,
                PixelDataType = PixelType.UnsignedInt
            });

            picking_buffer = new Framebuffer(ref info);
        }

        {
            var info = new Framebuffer.FramebufferInfo() {
                HasDepth   = true,
                HasStencil = true
            };
            
            info.addColorInfo(new Framebuffer.FramebufferInfo.AttachmentInfo() {
                InternalPixelFormat = PixelInternalFormat.Rgba,
                PixelFormat         = PixelFormat.Rgba,
                PixelDataType       = PixelType.UnsignedByte
            });

            framebuffer = new Framebuffer(ref info);
            viewport_texture_ptr = framebuffer.getTargetTexture(0);
        }

        player = new LegacyEditorPhysicsSprite(physics_world, type: BodyType.Dynamic) {
            Identifier = "Player",
            Texture    = player_texture
        };

        var test = new LegacyEditorPhysicsSprite(physics_world, new Vector2(3.0f, 0.25f), new Vector2(-1f, -2.0f), initial_rotation: -20.0f) {
            Identifier = "Platform",
            Color = Color4.Orange
        };

        var floor = new LegacyEditorPhysicsSprite(physics_world, new Vector2(10.0f, 0.25f), new Vector2(0.0f, -5.0f)) {
            Identifier = "Floor"
        };

        sprites.Add(player);
        sprites.Add(test);
        sprites.Add(floor);

        RenderManager.OnResize += picking_buffer.resize;
        RenderManager.OnResize += imgui_controller.WindowResized;
        RenderManager.OnResize += camera.resize;
        
        ApplicationManager.Instance.TextInput  += textInput;
        ApplicationManager.Instance.MouseWheel += mouseWheel;

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        
        PhysicsManager.addWorld(physics_world);
    }
    public override void destroy() {
        Console.WriteLine("Destroyed TestScene!");
        RenderManager.OnResize -= picking_buffer.resize;
        RenderManager.OnResize -= imgui_controller.WindowResized;
        RenderManager.OnResize -= camera.resize;

        ApplicationManager.Instance.TextInput  -= textInput;
        ApplicationManager.Instance.MouseWheel -= mouseWheel;
        
        ImGui.SaveIniSettingsToDisk("test_scene");

        imgui_controller.Dispose();
    }

    public override void render() {
        renderPicking();
        
        framebuffer.bind();
        GL.Enable(EnableCap.StencilTest);
        GL.Viewport(0, 0, (int) viewport_size.X, (int) viewport_size.Y);
        RenderManager.clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        
        GL.StencilMask(0x00);
        
        batch.begin(camera);
        
        // Render all other sprites
        foreach (var sprite in sprites.Where(sprite => sprite != selected))
            sprite.render(batch);

        batch.end();

        GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
        GL.StencilMask(0xFF);
        
        batch.begin(camera);

        // Render Selected sprite (which effects the stencil buffer
        selected?.render(batch);

        batch.end();

        GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
        GL.StencilMask(0x00);
        
        if (selected != null && render_outline) {
            outline_shader.bind();
            outline_shader.setUniform("uOutlineColor", outline_color);
            
            picking_batch.begin(camera, outline_shader);
            ((ILegacyEditorSprite) selected).renderOutline(picking_batch, outline_size);
            picking_batch.end();
        }
        
        GL.StencilMask(0xFF);
        GL.StencilFunc(StencilFunction.Always, 0, 0xFF);
        
        Framebuffer.unbind();
        
        GL.Viewport(0, 0, RenderManager.WindowWidth, RenderManager.WindowHeight);
        imgui_controller.Render();
        ImGuiController.CheckGLError("End of frame");
    }
    private void renderPicking() {
        picking_buffer.bind(FramebufferTarget.DrawFramebuffer);
        var old_color = RenderManager.ClearColor;
        GL.Viewport(0, 0, (int) viewport_size.X, (int) viewport_size.Y);
        RenderManager.ClearColor = Color4.Black;
        RenderManager.clear();
        
        picking_batch.begin(camera);
        
        for (var i = 0; i < sprites.Count; i++) {
            var sprite = sprites[i];
            ((ILegacyEditorSprite) sprite).renderPicking(picking_batch, i + 1);
        }

        picking_batch.end();
        Framebuffer.unbind(FramebufferTarget.DrawFramebuffer);
        
        RenderManager.ClearColor = old_color;
    }

    private bool positionInViewport(Vector2 mouse_position) => mouse_position.isBetweenOrEqual(viewport_offset.toVector2(), (viewport_offset + viewport_size).toVector2());

    
    public override void update() {
        imgui_controller.Update(ApplicationManager.Instance, Time.DeltaTime);
        updateImGui();

        var position = ViewportMousePosition;
        var in_viewport = positionInViewport(InputManager.MousePosition) && can_focus_viewport;
        
        if (InputManager.mouseRelease(MouseButton.Right) && in_viewport) {
            if (selected != null) {
                selected.Position = camera.screenToWorldSpace(position);

                if (selected is LegacyEditorPhysicsSprite physics_selected) {
                    physics_selected.Body.LinearVelocity  = PhysicsVector2.Zero;
                    physics_selected.Body.AngularVelocity = 0.0f;
                    physics_selected.Body.Awake = true;
                }
            }
        }

        if (InputManager.mouseRelease(MouseButton.Left) && in_viewport) {
            var info = picking_buffer.readPixel<PickingPixelInfo>((int)position.X, (int) viewport_size.Y - (int) position.Y, PixelFormat.RgbInteger, PixelType.UnsignedInt);
            
            select(info.ElementIndex > 0 ? sprites[(int) info.ElementIndex - 1] : null);
        }

        if (InputManager.mouseDown(MouseButton.Middle) && viewport_focused) {
            var move = InputManager.MouseDelta * Time.DeltaTime * CAMERA_MOVE_SPEED;
            camera.Position += new Vector2(-move.X, move.Y);
        }

        foreach (var sprite in sprites)
            sprite.update();
    }
    public override void fixedUpdate() {
        foreach (var sprite in sprites)
            sprite.fixedUpdate();
    }
    public override void lateUpdate() {
        foreach (var sprite in sprites)
            sprite.lateUpdate();
    }
    
    private static readonly Vector4[] vertices = {
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(-0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f,  0.5f, 0.0f, 1.0f),
        new( 0.5f, -0.5f, 0.0f, 1.0f)
    };

    private void updateImGui() {
        ImGui.DockSpaceOverViewport();

        ImGui.ShowDemoWindow();

        ImGui.Begin("Viewport");

        old_viewport_size   = viewport_size;
        old_viewport_offset = viewport_offset;

        viewport_content_offset = ImGui.GetWindowContentRegionMin();
        viewport_offset         = ImGui.GetWindowPos() + viewport_content_offset;
        viewport_size           = ImGui.GetWindowContentRegionMax() - viewport_content_offset;

        viewport_focused = ImGui.IsWindowFocused();
        ImGui.Image(viewport_texture_ptr, viewport_size, VIEWPORT_UV_0, VIEWPORT_UV_1);

        updateViewport();

        if (selected != null) {
            // Transform tool stuff
            
            var list = ImGui.GetWindowDrawList();
            
            var pos  = (camera.worldToScreenSpace(selected.Position)).toNativeVector2() + viewport_offset;
            var size = camera.worldScaleToScreenScale(selected.Scale).toNativeVector2();

            var transform = Matrix4.CreateScale(new Vector3(size.X, size.Y, 1.0f))
                            * Matrix4.CreateRotationZ(-MathHelper.DegreesToRadians(selected.Rotation))
                            * Matrix4.CreateTranslation(new Vector3(pos.X, pos.Y, 0.0f));

            var bl = vertices[0] * transform;
            var tl = vertices[1] * transform;
            var tr = vertices[2] * transform;
            var br = vertices[3] * transform;

            NativeVector2[] selected_vertices = {
                new(bl.X, bl.Y),
                new(tl.X, tl.Y),
                new(tr.X, tr.Y),
                new(br.X, br.Y),
                new(bl.X, bl.Y)
            };
            
            list.AddPolyline(ref selected_vertices[0], 5, 0xffff00ff, ImDrawFlags.None, 2.0f);

            /*var pos = (camera.worldToScreenSpace(selected.Position)).toNativeVector2() + viewport_offset;
            var half_size = camera.worldScaleToScreenScale(selected.Scale / 2.0f).toNativeVector2();
            
            list.AddRect(pos - half_size, pos + half_size, 0xffff00ff);*/


        }

        ImGui.End();

        ImGui.Begin("Inspector");

        var selected_type = ((ILegacyEditorSprite?) selected)?.EditorTypeName ?? "None";
        ImGui.Text($"Selected: {selected_type}");
        ImGui.Separator();

        ((ILegacyEditorSprite?) selected)?.drawInspectable();

        ImGui.End();

        ImGui.Begin("Scene");

        var popup_open = false;
        
        for (var i = 0; i < sprites.Count; i++) {
            var sprite = sprites[i];
            if (ImGui.Selectable($"{sprite.Identifier}##{i}", sprite == selected))
                select(sprite);

            if (!ImGui.BeginPopupContextItem()) 
                continue;
            
            popup_open = true;
                
            if (ImGui.MenuItem("Delete"))
                destroy(sprite);
                
            ImGui.EndPopup();
        }

        if (!popup_open && ImGui.BeginPopupContextWindow()) {
            if (ImGui.MenuItem("Create Sprite"))
                sprites.Add(new LegacyEditorSprite());
            if (ImGui.MenuItem("Create Physics Sprite"))
                sprites.Add(new LegacyEditorPhysicsSprite(physics_world, type: BodyType.Dynamic));
            
            ImGui.EndPopup();
        }
        
        ImGui.End();
    }
    private void updateViewport() {
        if (old_viewport_size == viewport_size)
            return;
        
        framebuffer.resize((int) viewport_size.X, (int) viewport_size.Y);
        picking_buffer.resize((int) viewport_size.X, (int) viewport_size.Y);
        camera.resize((int) viewport_size.X, (int) viewport_size.Y);
    }
    
    private void select(SpriteObject? sprite) => selected = sprite;

    private void destroy(SpriteObject sprite) {
        sprite.destroy();
        sprites.Remove(sprite);
        
        if (selected == sprite)
            selected = null;
    }
    
    private void textInput(TextInputEventArgs e)   { imgui_controller.PressChar((char) e.Unicode); }
    private void mouseWheel(MouseWheelEventArgs e) { imgui_controller.MouseScroll(e.Offset); }
}

public struct LegacyPickingPixelInfo {
    public uint ElementIndex   { get; private set; } = 0;
    public uint Unused         { get; private set; } = 0;
    public uint PrimitiveIndex { get; private set; } = 0;
        
    public LegacyPickingPixelInfo() { }
}