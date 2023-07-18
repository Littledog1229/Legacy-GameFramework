using OpenTK.Mathematics;

namespace ApplicationCore.Render.Camera; 

// TODO: Hide initialize and resize
public abstract class Camera {
    public Matrix4 View       { get; protected set; }
    public Matrix4 Projection { get; protected set; }
    
    public int Width  { get; private set; }
    public int Height { get; private set; }
    
    public void initialize() {
        resize(RenderManager.WindowWidth, RenderManager.WindowHeight);
        createView();
    }

    public void resize(int width, int height) {
        Width  = width;
        Height = height;
        
        createProjection();
    }

    protected abstract void createView();
    protected abstract void createProjection();
}