using System.Drawing;
using System.Reflection;
using ApplicationCore.Application;
using ApplicationCore.Render.Buffer;
using ApplicationCore.Render.Texture;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ApplicationCore.Render; 

public static class RenderManager {
    public static int    WindowWidth  { get; private set; }
    public static int    WindowHeight { get; private set; }
    public static Color4 ClearColor   { set => GL.ClearColor(value); }

    public static Action<int, int>? OnResize { get; set; } = null;

    public static void setClearColor(Color color)  => GL.ClearColor(color);
    public static void setClearColor(Color4 color) => GL.ClearColor(color);
    public static void setClearColor(float r, float g, float b, float a = 1.0f) => GL.ClearColor(r, g, b, a);

    public static void clear(ClearBufferMask mask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit) => GL.Clear(mask);

    public static Shader getOrCreateShader(string identifier, string filepath, Assembly? assembly) {
        if (!shaders.TryGetValue(identifier, out var shader))
            shader = new Shader(identifier, filepath, assembly);

        return shader;
    }
    
    private static List<VertexBuffer>         vertex_buffers = new();
    private static List<IndexBuffer>          index_buffers  = new();
    private static List<VertexArray>          vertex_arrays  = new();
    private static Dictionary<string, Shader> shaders        = new();
    private static List<Texture2D>            texture_2ds    = new();

    private static List<Camera.Camera> cameras = new();

    internal static void registerVertexBuffer (VertexBuffer buffer)              => vertex_buffers.Add(buffer);
    internal static void registerIndexBuffer  (IndexBuffer buffer)               => index_buffers.Add(buffer);
    internal static void registerVertexArray  (VertexArray array)                => vertex_arrays.Add(array);
    internal static void registerShader       (string identifier, Shader shader) => shaders.Add(identifier, shader);
    internal static void registerTexture2D    (Texture2D texture)                => texture_2ds.Add(texture);

    internal static void deregisterVertexBuffer (VertexBuffer buffer) => vertex_buffers.Remove(buffer);
    internal static void deregisterIndexBuffer  (IndexBuffer buffer)  => index_buffers.Remove(buffer);
    internal static void deregisterVertexArray  (VertexArray array)   => vertex_arrays.Remove(array);
    internal static void deregisterShader       (string identifier)   => shaders.Remove(identifier);
    internal static void deregisterTexture2D    (Texture2D texture)   => texture_2ds.Remove(texture);

    internal static void registerCamera(Camera.Camera camera) => cameras.Add(camera);

    internal static void deregisterCamera(Camera.Camera camera) => cameras.Remove(camera);

    internal static void resize(int width, int height) {
        WindowWidth  = width;
        WindowHeight = height;
        
        GL.Viewport(0, 0, width, height);
        
        foreach (var camera in cameras)
            camera.resize();

        OnResize?.Invoke(width, height);
    }
    
    internal static void initialize() { }
    internal static void start()      { }
    internal static void unload()     { }

    internal static void destroy() {
        Console.WriteLine("RenderManager - Disposing Objects:");
        Console.WriteLine($" . Vertex Arrays:  {vertex_arrays.Count}");
        Console.WriteLine($" . Vertex Buffers: {vertex_buffers.Count}");
        Console.WriteLine($" . Index Buffers:  {index_buffers.Count}");
        Console.WriteLine($" . Shaders:        {shaders.Count}");
        Console.WriteLine($" . Texture2Ds:     {texture_2ds.Count}");
        
        foreach (var array       in vertex_arrays)  { array.internalDestroy();     }
        foreach (var buffer      in vertex_buffers) { buffer.internalDestroy();    }
        foreach (var buffer      in index_buffers)  { buffer.internalDestroy();    }
        foreach (var (_, shader) in shaders)        { shader.internalDestroy();    }
        foreach (var texture2d   in texture_2ds)    { texture2d.internalDestroy(); }
        
        vertex_arrays.Clear();
        vertex_buffers.Clear();
        index_buffers.Clear();
        shaders.Clear();
        texture_2ds.Clear();
    }

    internal static void preRender() {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        clear();
    }
    internal static void render()     { }
    internal static void postRender() { }
}