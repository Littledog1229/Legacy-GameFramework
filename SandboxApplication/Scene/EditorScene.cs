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
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sandbox.Editor;
using Sandbox.Editor.CreationContexts;
using Sandbox.Editor.Inspectors;
using Sandbox.Editor.Tools;
using Sandbox.Serialization;
using Sandbox.Sprite;
using tainicom.Aether.Physics2D.Dynamics;

using NativeVector2  = System.Numerics.Vector2;
using PhysicsVector2 = tainicom.Aether.Physics2D.Common.Vector2;

namespace Sandbox; 

// Next up:
// TODO: Serialize and Deserialize Scene
// TODO: Click and drag sprites in scene view to re-order them
// InProgress: Sprite Editing Tools (Move, Scale, Rotate, etc... [all definable by the sprites type])
//    . Finished: Transform  Tool
//    . Finished: Rotation   Tool
//    . Finished: Scale      Tool
//    . TODO:     Manipulate Tool [basically, Unity's box thing where you can move and adjust the bounding box]
// TODO: Texture Manager / Atlas Manager

// Far off:
// TODO: ECS based sprites for easier editing

public sealed class EditorScene : Scene {
    private static readonly NativeVector2 VIEWPORT_UV_0 = new(0.0f, 1.0f);
    private static readonly NativeVector2 VIEWPORT_UV_1 = new(1.0f, 0.0f);
    
    //private static readonly Dictionary<Type, >

    #region Render Objects
    private readonly Shader       outline_shader  = RenderManager.getOrCreateShader("Outline", "Resource.Shader.Outline.glsl");
    private readonly Texture2D    player_texture  = new("Resource.Texture.temp_player.png");
    private readonly ShapeBatch   render_batch    = new();
    private readonly PickingBatch picking_batch   = new();
    private          Framebuffer  render_buffer   = null!;
    private          Framebuffer  picking_buffer  = null!;
    #endregion
    
    #region Scene Objects
    public readonly List<EditorSprite> Sprites      = new();
    public readonly OrthoCamera        Camera       = new();
    public readonly World              PhysicsWorld = new();

    private bool run_physics = true;
    #endregion
    
    #region Editor Objects
    private readonly ImGuiController                   imgui_controller = new(RenderManager.WindowWidth, RenderManager.WindowHeight);
    private readonly Dictionary<Type, string>          editor_names     = new() {
        { typeof(SpriteObject),  "Sprite" },
        { typeof(PhysicsSprite), "PhysicsSprite" },
    };
    private readonly Dictionary<Type, CustomInspector> inspectors       = new() {
        { typeof(SpriteObject),  new DefaultInspector() },
        { typeof(PhysicsSprite), new PhysicsSpriteInspector() }
    };
    private readonly Dictionary<Type, CreationContext> contexts         = new();
    private readonly Dictionary<string, EditorTool>    tools            = new();

    private readonly List<EditorTool> valid_tools = new();

    private string                 scene_name         = string.Empty;
    private string                 current_scene_path = string.Empty;
    private string                 load_scene_path    = string.Empty;
    private string                 save_scene_path    = string.Empty;
    private string                 scene_path         = string.Empty;
    private bool                   scene_loaded       = false;
    private bool                   scene_dirty        = true;
    
    private EditorSprite?          selected_sprite;
    private SpriteCreationContext? sprite_create_context;
    private EditorTool?            current_tool;

    public bool CanUpdateSelected = true;

    private bool open_sprite_create_context;

    private const float CAMERA_MOVE_SPEED = 1.0f;

    
    #region Viewport Objects
    public NativeVector2  ViewportSize;
    public NativeVector2  ViewportOffset;
    public NativeVector2  ViewportContentOffset;
    private NativeVector2 viewport_size_old;
    private IntPtr        viewport_texture_ptr;
    private bool          can_focus_viewport = true;
    private bool          viewport_focused;
    #endregion
    
    private Vector2 outline_size   = new(0.075f, 0.075f);
    private Color4  outline_color  = Color4.OrangeRed;
    private bool    render_outline = true;
    #endregion
    
    #region Viewport Helper Methods
    public Vector2 ViewportMousePosition => InputManager.MousePosition - new Vector2(ViewportOffset.X, ViewportOffset.Y);
    #endregion
    
    #region Scene Methods
    public override void start() {
        ImGui.LoadIniSettingsFromDisk("EditorScene.imgui.ini");

        // Setup Objects
        Camera.ViewSizeX = 10.0f;
        render_batch.initialize();
        picking_batch.initialize();
        Camera.initialize();

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

        // Setup Editor
        contexts.Add(typeof(SpriteObject),  new SpriteCreationContext());
        contexts.Add(typeof(PhysicsSprite), new PhysicsSpriteCreationContext(PhysicsWorld));
        
        tools.Add("Transform", new TransformTool(this));
        tools.Add("Rotation",  new RotationTool(this));
        tools.Add("Scale",     new ScaleTool(this));

        current_tool = tools["Transform"];
        
        valid_tools.Add(tools["Transform"]);
        valid_tools.Add(tools["Rotation"]);
        valid_tools.Add(tools["Scale"]);
        
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
        var position = ViewportMousePosition;
        var in_viewport = inViewport(InputManager.MousePosition) && can_focus_viewport;
        
        if (InputManager.mouseRelease(MouseButton.Right) && in_viewport) {
            if (selected_sprite != null) {
                selected_sprite.Sprite.Position = Camera.screenToWorldSpace(position);

                if (selected_sprite.Sprite is PhysicsSprite physics_selected) {
                    physics_selected.Body.LinearVelocity  = PhysicsVector2.Zero;
                    physics_selected.Body.AngularVelocity = 0.0f;
                    physics_selected.Body.Awake = true;
                }
            }
        }

        if (InputManager.mouseRelease(MouseButton.Left) && in_viewport && CanUpdateSelected) {
            var info = picking_buffer.readPixel<PickingPixelInfo>((int)position.X, (int) ViewportSize.Y - (int) position.Y, PixelFormat.RgbInteger, PixelType.UnsignedInt);
            
            select(info.ElementIndex > 0 ? Sprites[(int) info.ElementIndex - 1] : null);
        }

        if (InputManager.mouseDown(MouseButton.Middle) && in_viewport) {
            var move = InputManager.MouseDelta * Time.DeltaTime * CAMERA_MOVE_SPEED;
            Camera.Position += new Vector2(-move.X, move.Y);
        }
        
        foreach (var sprite in Sprites)
            sprite.update();
        
        // Update ImGui
        imgui_controller.Update(ApplicationManager.Instance, Time.DeltaTime);
        updateImgui();
    }
    public override void fixedUpdate() {
        if (!run_physics)
            return;
        
        PhysicsWorld.Step(Time.FixedDeltaTime);

        foreach (var sprite in Sprites)
            sprite.fixedUpdate();
    }
    public override void lateUpdate() {
        foreach (var sprite in Sprites)
            sprite.lateUpdate();
    }

    #region Rendering
    public override void render() {
        renderPicking();
        
        #region Render Scene
        // Enable stencil testing
        GL.Enable(EnableCap.StencilTest);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        
        // Clear the Framebuffer
        render_buffer.bind();
        GL.Viewport(0, 0, (int) ViewportSize.X, (int) ViewportSize.Y);
        RenderManager.clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        
        GL.StencilMask(0x00); // Dont change the stencil mask

        // Render all sprites (besides the selected sprite)
        render_batch.begin(Camera);
        foreach (var sprite in Sprites.Where(sprite => sprite != selected_sprite))
            sprite.render(render_batch);
        render_batch.end();
        
        // Render the selected sprite and its outline (if applicable)
        if (selected_sprite != null) {
            // Update the stencil mask (for outlining the selected sprite)
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);
            
            // Render the selected sprite
            render_batch.begin(Camera);
            selected_sprite.render(render_batch);
            render_batch.end();

            // Update the stencil state to render only the outline
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilMask(0x00);
            
            // Render the outline
            if (render_outline) {
                outline_shader.bind();
                outline_shader.setUniform("uOutlineColor", outline_color);
            
                picking_batch.begin(Camera, outline_shader);
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

        GL.Viewport(0, 0, (int) ViewportSize.X, (int) ViewportSize.Y);
        RenderManager.clear();
        
        picking_batch.begin(Camera);

        for (var i = 0; i < Sprites.Count; i++) {
            var sprite = Sprites[i];
            sprite.renderPicking(picking_batch, i + 1);
        }
        
        picking_batch.end();
        Framebuffer.unbind(FramebufferTarget.DrawFramebuffer);

        RenderManager.ClearColor = old_color;
    }
    #endregion

    public string serialize() {
        var settings = new JsonSerializerSettings() {
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return JsonConvert.SerializeObject(Sprites.Select(editor_sprite => editor_sprite.Sprite.serialize()).ToList(), Formatting.Indented, settings);
    }

    public void deserialize(string path) {
        using var file_stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        using var file_reader = new StreamReader(file_stream);
        
        //Sprites.AddRange();
    }
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
        updateSceneManager();
        updateInspector();
        updateSceneView();
        updateToolsWindow();
        updateSceneViewport();
    }

    private void updateEditorSettings() {
        ImGui.Begin("Editor Settings");

        ImGui.Checkbox("Run Physics", ref run_physics);
        
        ImGui.End();
    }
    private void updateSceneViewport() {
        var extra_flags = ImGuiWindowFlags.None;
        if (scene_dirty)
            extra_flags |= ImGuiWindowFlags.UnsavedDocument;
        
        ImGui.Begin("Scene Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | extra_flags);
        
        // Viewport Setup
        viewport_size_old = ViewportSize;

        ViewportContentOffset = ImGui.GetWindowContentRegionMin();
        ViewportOffset        = ImGui.GetWindowPos() + ViewportContentOffset;
        ViewportSize          = ImGui.GetWindowContentRegionMax() - ViewportContentOffset;

        can_focus_viewport = ImGui.IsWindowHovered();
        viewport_focused   = ImGui.IsWindowFocused();
        ImGui.Image(viewport_texture_ptr, ViewportSize, VIEWPORT_UV_0, VIEWPORT_UV_1);

        // Viewport state update
        if (viewport_size_old != ViewportSize) {
            render_buffer.resize((int) ViewportSize.X, (int) ViewportSize.Y);
            picking_buffer.resize((int) ViewportSize.X, (int) ViewportSize.Y);
            Camera.resize((int) ViewportSize.X, (int) ViewportSize.Y);
        }
        
        // Render and handle tools
        current_tool?.updateWidget();
        
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

        for (var i = 0; i < Sprites.Count; i++) {
            var sprite = Sprites[i];
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
            updateSpriteCreate();
            
            ImGui.EndPopup();
        }

        if (open_sprite_create_context)
            ImGui.OpenPopup("SpriteContextCreate");
        
        updateCreateContext();

        ImGui.End();
    }
    private void updateToolsWindow() {
        ImGui.Begin("Editor Tools");

        var tool_name = current_tool == null ? "None" : current_tool.Identifier;
        ImGui.Text($"Current Tool: {tool_name}");
        ImGui.Separator();

        foreach (var tool in valid_tools.Where(tool => ImGui.Selectable(tool.Identifier, tool == current_tool)))
            setCurrentTool(tool);
        
        
        ImGui.End();
    }
    private void updateSceneManager() {
        var padding_x = ImGui.GetStyle().WindowPadding.X;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NativeVector2(0.0f));
        ImGui.Begin("Scene Manager");
        ImGui.PopStyleVar();

        ImGui.BeginTabBar("SceneManager");
        ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0.0f);
        
        if (ImGui.BeginTabItem("Load")) {
            ImGui.Indent(padding_x);

            ImGui.Text("Load Path");
            ImGui.SameLine();
            ImGui.InputText("##LoadScenePath", ref load_scene_path, uint.MaxValue);
            
            ImGui.Unindent(padding_x);
            ImGui.EndTabItem();
        }
        
        if (ImGui.BeginTabItem("Save")) {
            ImGui.Indent(padding_x);

            ImGui.Text("Save Path");
            ImGui.SameLine();
            ImGui.InputText("##SaveScenePath", ref save_scene_path, uint.MaxValue);

            if (ImGui.Button("Test Save")) {
                using var stream = new FileStream("test.txt", FileMode.OpenOrCreate);
                using var file_writer = new StreamWriter(stream);
                
                file_writer.Write(serialize());
            }
            
            ImGui.Unindent(padding_x);
            ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
        
        ImGui.PopStyleVar();
        ImGui.End();
    }

    private void updateSpriteCreate() {
        if (ImGui.MenuItem("Create Sprite"))
            setSpriteCreateContext(typeof(SpriteObject));

        if (ImGui.MenuItem("Create Physics Sprite"))
            setSpriteCreateContext(typeof(PhysicsSprite));
    }
    private void updateCreateContext() {
        if (open_sprite_create_context) {
            ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new NativeVector2(0.5f, 0.5f));
            ImGui.SetNextWindowSize(ImGui.GetMainViewport().Size * 0.25f, ImGuiCond.Appearing);
        }

        if (sprite_create_context == null || !ImGui.BeginPopupModal("SpriteContextCreate", ref open_sprite_create_context, ImGuiWindowFlags.NoSavedSettings)) {
            ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
            return;
        }

        var sprite_type_name = $"{editor_names[sprite_create_context!.SpriteType]}";
        var half_size = ImGui.CalcTextSize(sprite_type_name) / 2.0f;
        var modal_half_size = ImGui.GetWindowSize() / 2.0f;
        ImGui.SetCursorPos(new NativeVector2(modal_half_size.X - half_size.X, ImGui.GetCursorPosY()));
        ImGui.Text(sprite_type_name);
        ImGui.Separator();
        ImGui.AlignTextToFramePadding();
        sprite_create_context.renderImgui();
        ImGui.Separator();
        if (ImGui.Button("Create")) {
            ImGui.CloseCurrentPopup();
            createEditorSprite(sprite_create_context.create<SpriteObject>());
            setSpriteCreateContext(null);
        }
        
        ImGui.EndPopup();
    }

    private void imguiTextInput(TextInputEventArgs e)   { imgui_controller.PressChar((char) e.Unicode); }
    private void imguiMouseWheel(MouseWheelEventArgs e) { imgui_controller.MouseScroll(e.Offset); }
    #endregion
    
    #region Editor Methods
    private bool inViewport(Vector2 mouse_position) => mouse_position.isBetweenOrEqual(ViewportOffset.toVector2(), (ViewportOffset + ViewportSize).toVector2());

    private void select(EditorSprite? sprite) {
        selected_sprite = sprite;
        current_tool?.setSprite(sprite);
    }
    private void createEditorSprite(SpriteObject sprite) => Sprites.Add(new EditorSprite(this, sprite));
    private void destroyEditorSprite(EditorSprite sprite) {
        sprite.destroy();
        Sprites.Remove(sprite);

        if (selected_sprite == sprite)
            select(null);
    }
    private void setCurrentTool(EditorTool? tool) {
        current_tool?.setSprite(null);
        current_tool = tool;
        current_tool?.setSprite(selected_sprite);
    }

    private void setSpriteCreateContext(Type? type) {
        if (type == null) {
            open_sprite_create_context = false;
            sprite_create_context = null;
            return;
        }
        
        open_sprite_create_context = true;
        sprite_create_context      = contexts[type] as SpriteCreationContext;
        sprite_create_context?.reset();
    }
    #endregion
}

public struct PickingPixelInfo {
    public uint ElementIndex   { get; private set; } = 0;
    public uint Unused         { get; private set; } = 0;
    public uint PrimitiveIndex { get; private set; } = 0;
        
    public PickingPixelInfo() { }
}