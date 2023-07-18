using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ApplicationCore.Render; 

public sealed class Framebuffer : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.Framebuffer;

    public bool HasDepth   { get; }
    public bool HasStencil { get; }

    private readonly List<FramebufferInfo.AttachmentInfo> attachment_infos    = new();
    private readonly List<int>                            framebuffer_targets = new(); // Last (1 or 2) are always the depth and stencil (in that order, depending which are being used)

    public int getTargetTexture(int index) => framebuffer_targets[index];
    
    public Framebuffer(    FramebufferInfo info) : this(ref info) { }
    public Framebuffer(ref FramebufferInfo info) {
        ObjectHandle = GL.GenFramebuffer();
        bind();

        HasDepth   = info.HasDepth;
        HasStencil = info.HasStencil;
        
        createColorAttachments(info);
        createDepthAndStencilAttachments();
        
        resize(RenderManager.WindowWidth, RenderManager.WindowHeight);
        
        var ec = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (ec != FramebufferErrorCode.FramebufferComplete)
            Console.WriteLine($"Incomplete framebuffer: {ec}");

        RenderManager.registerFramebuffer(this);
    }
    
    public        void bind   (FramebufferTarget target = FramebufferTarget.Framebuffer) => GL.BindFramebuffer(target, ObjectHandle);
    public static void unbind (FramebufferTarget target = FramebufferTarget.Framebuffer) => GL.BindFramebuffer(target, 0);
    
    public static void clear(ClearBufferMask mask) => GL.Clear(mask);

    public override void destroy() {
        GL.DeleteFramebuffer(ObjectHandle);
        
        foreach (var target in framebuffer_targets)
            GL.DeleteTexture(target);
        
        RenderManager.deregisterFramebuffer(this);
    }

    public T readPixel<T>(int x, int y, PixelFormat format, PixelType type) where T : struct {
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, ObjectHandle);
        
        GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

        var pixel = new T();
        GL.ReadPixels(x, y, 1, 1, format, type, ref pixel);
        
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

        return pixel;
    }
    public T readPixel<T>(Vector2i position, PixelFormat format, PixelType type) where T : struct {
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, ObjectHandle);
        
        GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

        var pixel = new T();
        GL.ReadPixels(position.X, position.Y, 1, 1, format, type, ref pixel);
        
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

        return pixel;
    }
    
    internal void internalDestroy() {
        GL.DeleteFramebuffer(ObjectHandle);

        foreach (var target in framebuffer_targets)
            GL.DeleteTexture(target);
    }
    
    private void createColorAttachments(FramebufferInfo info) {
        for (var i = 0; i < info.color_info.Count; i++) {
            var attachment_info = info.color_info[i];

            var color_handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, color_handle);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, color_handle, 0);
            attachment_infos.Add(attachment_info);
            framebuffer_targets.Add(color_handle);
        }
    }

    private void createDepthAndStencilAttachments() {
        if (!HasDepth) 
            return;
        
        var depth_handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, depth_handle);

        if (HasStencil) {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, depth_handle, 0);
            attachment_infos.Add(new FramebufferInfo.AttachmentInfo { InternalPixelFormat = PixelInternalFormat.Depth24Stencil8, PixelFormat = PixelFormat.DepthStencil, PixelDataType = PixelType.UnsignedInt248 });
        } else {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depth_handle, 0);
            attachment_infos.Add(new FramebufferInfo.AttachmentInfo { InternalPixelFormat = PixelInternalFormat.DepthComponent, PixelFormat = PixelFormat.DepthComponent, PixelDataType = PixelType.Float });
        }
        
        framebuffer_targets.Add(depth_handle);
    }

    public void resize(int width, int height) {
        for (var i = 0; i < framebuffer_targets.Count; i++) {
            var target = framebuffer_targets[i];
            var info      = attachment_infos[i];
            
            GL.BindTexture(TextureTarget.Texture2D, target);
            GL.TexImage2D(TextureTarget.Texture2D, 0, info.InternalPixelFormat, width, height, 0, info.PixelFormat, info.PixelDataType, IntPtr.Zero);
        }
    }
    
    public struct FramebufferInfo {
        public struct AttachmentInfo {
            public PixelFormat         PixelFormat         { get; set; }
            public PixelInternalFormat InternalPixelFormat { get; set; }
            public PixelType           PixelDataType       { get; set; }

            public static readonly AttachmentInfo DEFAULT = new() { InternalPixelFormat = PixelInternalFormat.Rgba, PixelFormat = PixelFormat.Rgba, PixelDataType = PixelType.UnsignedByte };
        }
        
        public bool HasDepth   { get; set; } = true;
        public bool HasStencil { get; set; } = false;
        
        internal List<AttachmentInfo> color_info  = new();

        public FramebufferInfo() { }
        
        public void addColorInfo()                    => color_info.Add(AttachmentInfo.DEFAULT);
        public void addColorInfo(AttachmentInfo info) => color_info.Add(info);
    }
}