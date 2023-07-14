using OpenTK.Mathematics;

namespace ApplicationCore.Render.Camera; 

// TODO: Hide initialize and resize
public abstract class Camera {
    public Matrix4 View       { get; protected set; }
    public Matrix4 Projection { get; protected set; }

    protected Camera() { RenderManager.registerCamera(this); }

    public void destroy() => RenderManager.deregisterCamera(this);
    
    public void initialize() {
        createProjection();
        createView();
    }

    public void resize() {
        createProjection();
    }

    protected abstract void createView();
    protected abstract void createProjection();
}