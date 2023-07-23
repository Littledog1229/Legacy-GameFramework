using ApplicationCore.Application;
using ApplicationCore.Render;
using ApplicationCore.Render.Camera;
using Engine.Scenes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sandbox3D.Mesh;
using Sandbox3D.Render;

using NativeVector3 = System.Numerics.Vector3;

namespace Sandbox3D;

public struct VertexPositionColor {
    public Vector3 Position { get; set; }
    public Color4  Color    { get; set; }

    public VertexPositionColor(Vector3 position, Color4 color) {
        Position = position;
        Color    = color;
    }
}

public sealed class GeneratorScene : Scene {
    private VertexArray       array;
    private Shader            shader;
    private FocusedPerspectiveCamera camera;

    public override void start() {
        {
            var info = new VertexArray.VertexArrayInfo();
            info.pushAttribute<Vector3>(1, false);
            info.pushAttribute<Color4>(1, false);

            var parameters = new MeshGenerator.GenerationParameters() {
                Scale = new Vector3(1.0f, 3.0f, 1.0f),
                Pivot = Vector3.One / 2.0f
            };
            
            array = new VertexArray(ref info);

            var (vertex_positions, indices) = new CubeGenerator().generate(parameters);
            var vertices = new VertexPositionColor[] {
                // North Face
                new(vertex_positions[0], Color4.Blue),
                new(vertex_positions[1], Color4.Blue),
                new(vertex_positions[2], Color4.Blue),
                new(vertex_positions[3], Color4.Blue),
                
                // East Face
                new(vertex_positions[4], Color4.Red),
                new(vertex_positions[5], Color4.Red),
                new(vertex_positions[6], Color4.Red),
                new(vertex_positions[7], Color4.Red),
                
                // South Face
                new(vertex_positions[ 8], Color4.Blue),
                new(vertex_positions[ 9], Color4.Blue),
                new(vertex_positions[10], Color4.Blue),
                new(vertex_positions[11], Color4.Blue),
                
                // West Face
                new(vertex_positions[12], Color4.Red),
                new(vertex_positions[13], Color4.Red),
                new(vertex_positions[14], Color4.Red),
                new(vertex_positions[15], Color4.Red),
                
                // Top Face
                new(vertex_positions[16], Color4.LimeGreen),
                new(vertex_positions[17], Color4.LimeGreen),
                new(vertex_positions[18], Color4.LimeGreen),
                new(vertex_positions[19], Color4.LimeGreen),
                
                // Bottom Face
                new(vertex_positions[20], Color4.LimeGreen),
                new(vertex_positions[21], Color4.LimeGreen),
                new(vertex_positions[22], Color4.LimeGreen),
                new(vertex_positions[23], Color4.LimeGreen),
            };

            array.VertexBuffer.bufferData(vertices);
            array.IndexBuffer.bufferData(indices);
        }

        shader = new Shader("PositionColor", "PositionColor.glsl");

        camera = new FocusedPerspectiveCamera(new Vector3(0.0f, 0.0f, 10.0f));
        camera.initialize();

        RenderManager.OnResize += camera.resize;
    }

    private float pitch;
    private float yaw;
    private float zoom = 10.0f;

    private float pitch_speed = 0.5f;
    private float yaw_speed   = 0.5f;
    private float move_speed  = 1.0f;

    public override void update() {
        if (InputManager.ScrollOffsetY != 0.0f) {
            if (InputManager.ScrollOffsetY > 0.0f) {
                zoom /= 2.0f;
                zoom = MathF.Max(zoom, 1.0f);
            } else {
                zoom *= 2.0f;
                zoom = MathF.Min(zoom, 20.0f);
            }
        }

        var middle_down = InputManager.mouseDown(MouseButton.Middle);

        var mouse_delta = InputManager.MouseDelta;
        
        if (mouse_delta != Vector2.Zero) {
            if (middle_down) {
                pitch += -mouse_delta.Y * pitch_speed;
                pitch = Math.Clamp(pitch, -89.9f, 89.9f);

                yaw += -mouse_delta.X * yaw_speed;
                yaw %= 360.0f;
            }
        }
        
        var true_rot = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(yaw)) * Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(pitch));
        var rot_pos = true_rot * new Vector3(0.0f, 0.0f, zoom);

        camera.Position = rot_pos;
    }

    public override void render() {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        shader.bind();
        shader.setUniform("uProjection", camera.Projection);
        shader.setUniform("uView",       camera.View);
        
        array.bind();
        
        array.drawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
    }
}