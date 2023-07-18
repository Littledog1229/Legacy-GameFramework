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
using Sandbox.Editor;
using Sandbox.Editor.Inspectors;
using Sandbox.Sprite;
using tainicom.Aether.Physics2D.Dynamics;

using NativeVector2 = System.Numerics.Vector2;
using PhysicsVector2 = tainicom.Aether.Physics2D.Common.Vector2;

namespace Sandbox; 

// Next up:
// TODO: Serialize and Deserialize Scene
// TODO: Create Context Menu (for sprite properties)
// TODO: Click and drag sprites in scene view to re-order them
// TODO: Sprite Editing Tools (Move, Scale, Rotate, etc... [all definable by the sprites type])
// TODO: Texture Manager / Atlas Manager

// Far off:
// TODO: ECS based sprites for easier editing

public sealed class EditorScene : Scene {
    private static readonly NativeVector2 VIEWPORT_UV_0 = new(0.0f, 1.0f);
    private static readonly NativeVector2 VIEWPORT_UV_1 = new(1.0f, 0.0f);
    
    #region Render Objects
    private readonly Shader       outline_shader  = RenderManager.getOrCreateShader("Outline", "Resource.Shader.Outline.glsl");
    private readonly Texture2D    player_texture  = new("Resource.Texture.temp_player.png");
    private readonly ShapeBatch   render_batch    = new();
    private readonly PickingBatch picking_batch   = new();
    private          Framebuffer  render_buffer   = null!;
    private          Framebuffer  picking_buffer  = null!;
    #endregion
    
    #region Scene Objects
    private readonly List<EditorSprite> sprites       = new();
    private readonly OrthoCamera        camera        = new();
    private readonly World              physics_world = new();

    private bool run_physics = true;
    #endregion
    
    #region Editor Objects
    private readonly ImGuiController                   imgui_controller = new(RenderManager.WindowWidth, RenderManager.WindowHeight);
    private readonly Dictionary<Type, string>          editor_names     = new() {
        { typeof(SpriteObject), "Sprite" },
        { typeof(PhysicsSprite), "PhysicsSprite" },
    };
    private readonly Dictionary<Type, CustomInspector> inspectors       = new() {
        { typeof(SpriteObject),  new DefaultInspector() },
        { typeof(PhysicsSprite), new DefaultInspector() }
    };

    private string        scene_name      = string.Empty;
    private string        scene_path      = string.Empty;
    private EditorSprite? selected_sprite = null;
    
    private const float CAMERA_MOVE_SPEED = 1.0f;

    
    #region Viewport Objects
    private NativeVector2 viewport_size;
    private NativeVector2 viewport_offset;
    private NativeVector2 viewport_content_offset;
    private NativeVector2 viewport_size_old;
    private IntPtr        viewport_texture_ptr;
    private bool          can_focus_viewport = true;
    private bool          viewport_focused;
    #endregion
    
    private Vector2 outline_size   = new(0.075f, 0.075f);
    private Color4  outline_color  = Color4.OrangeRed;
    private bool    render_outline = false;
    #endregion
    
    #region Viewport Helper Methods
    private Vector2 ViewportMousePosition => InputManager.MousePosition - new Vector2(viewport_offset.X, viewport_offset.Y);
    #endregion
    
    #region Scene Methods
    public override void start() {
        ImGui.LoadIniSettingsFromDisk("EditorScene.imgui.ini");

        // Setup Objects
        camera.ViewSizeX = 10.0f;
        render_batch.initialize();
        picking_batch.initialize();
        camera.initialize();

        #region Picking Framebuffer
        {
            var info = new Framebuffer.FramebufferInfo {
                HasDepth = true
            };
            
            info.addColorInfo(new Framebuffer.FramebufferInfo.AttachmentInfo {
                InternalPixelFormat = PixelInternalFormat.Rgb32ui,
                PixelFormat         = PixelFormat.RgbInteger,
                PixelDataType       = PixelType.UnsignedInt
            });

            picking_buffer = new Framebuffer(ref info);
        }
        #endregion
        #region Render Framebuffer
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

            render_buffer        = new Framebuffer(ref info);
            viewport_texture_ptr = render_buffer.getTargetTexture(0);
        }
        #endregion

        // Bind delegates
        RenderManager.OnResize += imgui_controller.WindowResized;

        ApplicationManager.Instance.TextInput  += imguiTextInput;
        ApplicationManager.Instance.MouseWheel += imguiMouseWheel;

        setupImgui();
    }
    public override void destroy() {
        // Unbind delegates
        RenderManager.OnResize -= imgui_controller.WindowResized;

        ApplicationManager.Instance.TextInput  -= imguiTextInput;
        ApplicationManager.Instance.MouseWheel -= imguiMouseWheel;
        
        // Destroy Render Objects
        outline_shader.destroy();
        player_texture.destroy();
        render_batch.destroy();
        picking_batch.destroy();
        render_buffer.destroy();
        picking_buffer.destroy();
        
        // Finalize ImGui
        ImGui.SaveIniSettingsToDisk("EditorScene.imgui.ini");
        imgui_controller.Dispose();
    }

    public override void update() {
        // Update ImGui
        imgui_controller.Update(ApplicationManager.Instance, Time.DeltaTime);
        updateImgui();
        
        var position = ViewportMousePosition;
        var in_viewport = inViewport(InputManager.MousePosition) && can_focus_viewport;
        
        if (InputManager.mouseRelease(MouseButton.Right) && in_viewport) {
            if (selected_sprite != null) {
                selected_sprite.Sprite.Position = camera.screenToWorldSpace(position);

                if (selected_sprite.Sprite is PhysicsSprite physics_selected) {
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

        if (InputManager.mouseDown(MouseButton.Middle) && in_viewport) {
            var move = InputManager.MouseDelta * Time.DeltaTime * CAMERA_MOVE_SPEED;
            camera.Position += new Vector2(-move.X, move.Y);
        }
        
        foreach (var sprite in sprites)
            sprite.update();
    }
    public override void fixedUpdate() {
        if (!run_physics)
            return;
        
        physics_world.Step(Time.FixedDeltaTime);

        foreach (var sprite in sprites)
            sprite.fixedUpdate();
    }
    public override void lateUpdate() {
        foreach (var sprite in sprites)
            sprite.lateUpdate();
    }

    #region Rendering
    public override void render() {
        renderPicking();
        
        #region Render Scene
        // Enable stencil testing
        GL.Enable(EnableCap.StencilTest);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        GL.StencilMask(0x00); // Dont change the stencil mask
        
        // Clear the Framebuffer
        render_buffer.bind();
        GL.Viewport(0, 0, (int) viewport_size.X, (int) viewport_size.Y);
        RenderManager.clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        
        
        // Render all sprites (besides the selected sprite)
        render_batch.begin(camera);
        foreach (var sprite in sprites.Where(sprite => sprite != selected_sprite))
            sprite.render(render_batch);
        render_batch.end();
        
        // Render the selected sprite and its outline (if applicable)
        if (selected_sprite != null) {
            // Update the stencil mask (for outlining the selected sprite)
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);
            
            // Render the selected sprite
            render_batch.begin(camera);
            selected_sprite.render(render_batch);
            render_batch.end();

            // Update the stencil state to render only the outline
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilMask(0x00);
            
            // Render the outline
            if (render_outline) {
                outline_shader.bind();
                outline_shader.setUniform("uOutlineColor", outline_color);
            
                picking_batch.begin(camera, outline_shader);
                selected_sprite.renderOutline(picking_batch, outline_size);
                picking_batch.end();
            }
        }
        
        // Reset the stencil state
        GL.StencilMask(0xFF);
        GL.StencilFunc(StencilFunction.Always, 0, 0xFF);

        // End rendering the viewport
        Framebuffer.unbind();
        
        // Render ImGui
        GL.Viewport(0, 0, RenderManager.WindowWidth, RenderManager.WindowHeight);
        imgui_controller.Render();
        ImGuiController.CheckGLError("End of frame");
        #endregion
    }
    private void renderPicking() {
        // TODO: Review, Update, and Comment
        picking_buffer.bind(FramebufferTarget.DrawFramebuffer);
        var old_color = RenderManager.ClearColor;
        RenderManager.ClearColor = Color4.Black;

        GL.Viewport(0, 0, (int) viewport_size.X, (int) viewport_size.Y);
        RenderManager.clear();
        
        picking_batch.begin(camera);

        for (var i = 0; i < sprites.Count; i++) {
            var sprite = sprites[i];
            sprite.renderPicking(picking_batch, i + 1);
        }
        
        picking_batch.end();
        Framebuffer.unbind(FramebufferTarget.DrawFramebuffer);

        RenderManager.ClearColor = old_color;
    }
    #endregion
    #endregion
    
    #region ImGui Methods
    private void setupImgui() {
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
    }

    private void updateImgui() {
        ImGui.DockSpaceOverViewport();
        
        ImGui.ShowDemoWindow();

        updateEditorSettings();
        updateViewport();
        updateInspector();
        updateSceneView();
    }

    private void updateEditorSettings() {
        ImGui.Begin("Editor Settings");

        ImGui.Checkbox("Run Physics", ref run_physics);
        
        ImGui.End();
    }

    private void updateViewport() {
        ImGui.Begin("Viewport");
        
        // Viewport Setup
        viewport_size_old = viewport_size;

        viewport_content_offset = ImGui.GetWindowContentRegionMin();
        viewport_offset         = ImGui.GetWindowPos() + viewport_content_offset;
        viewport_size           = ImGui.GetWindowContentRegionMax() - viewport_content_offset;

        viewport_focused = ImGui.IsWindowFocused();
        ImGui.Image(viewport_texture_ptr, viewport_size, VIEWPORT_UV_0, VIEWPORT_UV_1);
        
        // Viewport state update
        if (viewport_size_old != viewport_size) {
            render_buffer.resize((int) viewport_size.X, (int) viewport_size.Y);
            picking_buffer.resize((int) viewport_size.X, (int) viewport_size.Y);
            camera.resize((int) viewport_size.X, (int) viewport_size.Y);
        }
        
        ImGui.End();
    }

    private void updateInspector() {
        ImGui.Begin("Inspector");

        if (selected_sprite != null) {
            var selected_type = editor_names[selected_sprite.Sprite.GetType()];
            ImGui.Text($"Selected: {selected_type}");
            ImGui.Separator();
        
            inspectors[selected_sprite.Sprite.GetType()].drawInspectable(selected_sprite!.Sprite);
        } else
            ImGui.Text("Selected: None");
        
        
        
        ImGui.End();
    }

    private void updateSceneView() {
        ImGui.Begin("Scene");

        var popup_open = false;
        
        for (var i = 0; i < sprites.Count; i++) {
            var sprite = sprites[i];
            if (ImGui.Selectable($"{sprite.Sprite.Identifier}##{i}", sprite == selected_sprite))
                select(sprite);

            if (!ImGui.BeginPopupContextItem()) 
                continue;
            
            popup_open = true;
                
            if (ImGui.MenuItem("Delete"))
                destroyEditorSprite(sprite);
                
            ImGui.EndPopup();
        }

        if (!popup_open && ImGui.BeginPopupContextWindow()) {
            if (ImGui.MenuItem("Create Sprite"))
                createEditorSprite(new SpriteObject());
            if (ImGui.MenuItem("Create Physics Sprite"))
                createEditorSprite(new PhysicsSprite(physics_world, type: BodyType.Dynamic));
            
            ImGui.EndPopup();
        }
        
        ImGui.End();
    }

    private void imguiTextInput(TextInputEventArgs e)   { imgui_controller.PressChar((char) e.Unicode); }
    private void imguiMouseWheel(MouseWheelEventArgs e) { imgui_controller.MouseScroll(e.Offset); }
    #endregion
    
    #region Editor Methods
    private bool inViewport(Vector2 mouse_position) => mouse_position.isBetweenOrEqual(viewport_offset.toVector2(), (viewport_offset + viewport_size).toVector2());

    private void select(EditorSprite? sprite) => selected_sprite = sprite;
    private void createEditorSprite(SpriteObject sprite) => sprites.Add(new EditorSprite(this, sprite));
    private void destroyEditorSprite(EditorSprite sprite) {
        sprite.destroy();
        sprites.Remove(sprite);

        if (selected_sprite == sprite)
            selected_sprite = null;
    }
    #endregion
}

public struct PickingPixelInfo {
    public uint ElementIndex   { get; private set; } = 0;
    public uint Unused         { get; private set; } = 0;
    public uint PrimitiveIndex { get; private set; } = 0;
        
    public PickingPixelInfo() { }
}