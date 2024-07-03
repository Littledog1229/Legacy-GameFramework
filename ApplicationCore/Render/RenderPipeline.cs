using OpenTK.Graphics.OpenGL;

namespace ApplicationCore.Render; 

// TODO: Better Lifetime Management

// The main render state manager, controls how the application is rendered and when certain rendering events happen
// It is SUPPOSED to inherited by the client to control how they want their application to render
// All render methods (except for maybe the one in Application) will be removed in favor of this
public abstract class RenderPipeline {
    protected List<Action<RenderPipeline>> RenderStages { get; } = new();

    protected Stack<BoundFramebuffer> FramebufferStack  { get;      } = new();
    protected BoundFramebuffer?       ActiveFramebuffer { get; set; }

#if DEBUG
    // The amount of framebuffers in the stack at the beginning of each render frame (and the end)
    // Ensures that there is the correct amount of framebuffers at the start, and throws an exception if there isn't (in debug)
    protected virtual int StaticFramebufferCount { get; set; } = 0;
#endif
    
    // Pushes a new framebuffer to the framebuffer stack and sets it as the active framebuffer (discards the currently active framebuffer)
    public void pushFramebuffer(Framebuffer framebuffer, FramebufferTarget target = FramebufferTarget.Framebuffer) {
        var bound_framebuffer = new BoundFramebuffer(framebuffer, target);
        FramebufferStack.Push(bound_framebuffer);
        ActiveFramebuffer     = bound_framebuffer;
        
        framebuffer.bind();
    }
    // Pops a framebuffer from the framebuffer stack, and sets the next in line as the ActiveFramebuffer
    public void popFramebuffer() {
        var framebuffer = FramebufferStack.Pop();
        
        if (FramebufferStack.Count > 0) {
            ActiveFramebuffer = FramebufferStack.Peek();
            ActiveFramebuffer.Value.Framebuffer.bind(ActiveFramebuffer.Value.Target);
        } else
            Framebuffer.unbind(framebuffer.Target);
    }
    // Pushes both the ActiveFramebuffer and this new Framebuffer into the stack (ActiveFramebuffer then new framebuffer) and sets the new framebuffer as the ActiveFramebuffer
    public void pushActiveFramebuffer(Framebuffer framebuffer, FramebufferTarget target = FramebufferTarget.Framebuffer) {
        if (ActiveFramebuffer != null)
            FramebufferStack.Push(ActiveFramebuffer.Value);
        
        pushFramebuffer(framebuffer, target);
    }

    protected internal virtual void initialize() { }

    protected internal virtual void preparePresentationBuffer() {
        Framebuffer.unbind();
        
        GL.ClearColor(RenderManager.ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }
    
    protected internal virtual void render() {
#if DEBUG
        checkFramebufferStackCount();
#endif
        
        ActiveFramebuffer?.Framebuffer.bind(ActiveFramebuffer.Value.Target);
        
        foreach (var stage in RenderStages)
            stage.Invoke(this);
        
        Framebuffer.unbind();
    }

#if DEBUG
    protected void checkFramebufferStackCount() {
        if (FramebufferStack.Count != StaticFramebufferCount)
            throw new Exception($"The FramebufferStack contains {(FramebufferStack.Count > StaticFramebufferCount ? "more" : "less")} framebuffers than expected: [Found {FramebufferStack.Count}, Expected {StaticFramebufferCount}]");
    }
#endif

    public struct BoundFramebuffer {
        public Framebuffer       Framebuffer { get; }
        public FramebufferTarget Target      { get; set; }

        public BoundFramebuffer(Framebuffer framebuffer, FramebufferTarget target) {
            Framebuffer = framebuffer;
            Target      = target;
        }
    }
}