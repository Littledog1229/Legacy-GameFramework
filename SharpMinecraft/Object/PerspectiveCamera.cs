using Engine.Render;
using Engine.Render.Camera;
using OpenTK.Mathematics;
using SharpMinecraft.Object;

namespace SharpMinecraft.Object; 

public sealed class PerspectiveCamera : Camera {
    public CameraTransform CameraTransform { get; } = new();

    public PerspectiveCamera() {
        CameraTransform.MatrixUpdated += onMatrixUpdate;
        createProjection();
        createView();
    }

    private void onMatrixUpdate(Matrix4 transform) => View = Matrix4.LookAt(CameraTransform.Position, CameraTransform.Position + CameraTransform.Forward, CameraTransform.UP);

    protected override void createView()       => View = Matrix4.LookAt(CameraTransform.Position, CameraTransform.Position + CameraTransform.Forward, CameraTransform.UP);
    protected override void createProjection() => Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), RenderManager.AspectRatio, 0.1f, 1000.0f);
}