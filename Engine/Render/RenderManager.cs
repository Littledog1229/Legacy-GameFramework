using System.Reflection;
using Engine.Application;
using Engine.Render.Buffer;
using Engine.Render.Texture;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Render; 

public sealed class RenderManager {
    private static RenderManager Instance { get; set; } = null!;
    
    public static int   WindowWidth  { get; set; }
    public static int   WindowHeight { get; set; }
    public static float AspectRatio => (float) WindowWidth / WindowHeight;

    public static void setClearColor(float r, float g, float b, float a) => GL.ClearColor(r, g, b, a);
    public static void setClearColor(Color4 color)                       => GL.ClearColor(color);
    public static void setClearColor(ref Color4 color)                   => GL.ClearColor(color);

    public static void clearFramebuffer(ClearBufferMask mask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit) => GL.Clear(mask);

    public static VertexArray  createVertexArray (ref VertexArray.VertexArrayInfo info) {
        var array = new VertexArray(ref info);
        Instance.vertex_arrays.Add(array);

        return array;
    }
    public static VertexBuffer createVertexBuffer() {
        var buffer = new VertexBuffer();
        Instance.vertex_buffers.Add(buffer);

        return buffer;
    }
    public static IndexBuffer  createIndexBuffer () {
        var buffer = new IndexBuffer();
        Instance.index_buffers.Add(buffer);

        return buffer;
    }
    public static Shader       createShader      (string identifier, string filepath, Assembly? assembly = null) {
        var shader = new Shader(filepath, assembly);
        Instance.shaders.Add(identifier, shader);

        return shader;
    }
    public static Texture2D    createTexture2D   (string filepath, Assembly? assembly = null) {
        var texture = new Texture2D(filepath, assembly);
        Instance.texture_2ds.Add(texture);

        return texture;
    }

    public static void destroyVertexArray(VertexArray array) {
        array.destroy();
        Instance.vertex_arrays.Remove(array);
    }
    public static void destroyVertexBuffer(VertexBuffer buffer) {
        buffer.destroy();
        Instance.vertex_buffers.Remove(buffer);
    }
    public static void destroyIndexBuffer(IndexBuffer buffer) {
        buffer.destroy();
        Instance.index_buffers.Remove(buffer);
    }
    public static void destroyShader(string identifier) {
        Instance.shaders[identifier].destroy();
        Instance.shaders.Remove(identifier);
    }
    public static void destroyTexture2D(Texture2D texture) {
        texture.destroy();
        Instance.texture_2ds.Remove(texture);
    }

    public static Shader getShader(string identifier) => Instance.shaders[identifier];

    public static Shader getOrCreateShader(string identifier, string filepath, Assembly? assembly = null) => Instance.shaders.TryGetValue(identifier, out var shader) ? shader : createShader(identifier, filepath, assembly);

    private readonly List<VertexArray>  vertex_arrays  = new();
    private readonly List<VertexBuffer> vertex_buffers = new();
    private readonly List<IndexBuffer>  index_buffers  = new();
    private readonly List<Texture2D>    texture_2ds    = new();

    private readonly List<Camera.Camera> cameras = new();

    private readonly Dictionary<string, Shader> shaders = new();

    internal static void initialize() {
        if (Instance != null!)
            return;

        Instance = new RenderManager();
    }

    internal static void resize(int width, int height) {
        GL.Viewport(0, 0, width, height);
        
        WindowWidth  = width;
        WindowHeight = height;
        
        foreach (var camera in Instance.cameras)
            camera.resize();
    }
    
    internal static void startFrame() => Instance.internalStartFrame();
    internal static void endFrame()   => Instance.internalEndFrame();
    internal static void destroy()    => Instance.internalDestroy();
    
    private void internalStartFrame() {
        clearFramebuffer();
    }

    private void internalEndFrame() {
        WindowManager.swapBuffers();
    }

    private void internalDestroy() {
        Console.WriteLine("RenderManager - Disposing Objects:");
        Console.WriteLine($" . Vertex Arrays:  {vertex_arrays.Count}");
        Console.WriteLine($" . Vertex Buffers: {vertex_buffers.Count}");
        Console.WriteLine($" . Index Buffers:  {index_buffers.Count}");
        Console.WriteLine($" . Shaders:        {shaders.Count}");
        Console.WriteLine($" . Texture2Ds:     {texture_2ds.Count}");
        
        foreach (var array       in vertex_arrays)  { array.destroy();     }
        foreach (var buffer      in vertex_buffers) { buffer.destroy();    }
        foreach (var buffer      in index_buffers)  { buffer.destroy();    }
        foreach (var (_, shader) in shaders)        { shader.destroy();    }
        foreach (var texture2d   in texture_2ds)    { texture2d.destroy(); }

        
        vertex_arrays.Clear();
        vertex_buffers.Clear();
        index_buffers.Clear();
        shaders.Clear();
        texture_2ds.Clear();
    }
    
    internal static void addCamera(Camera.Camera camera) => Instance.cameras.Add(camera);
}