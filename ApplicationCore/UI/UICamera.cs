using ApplicationCore.Render;
using ApplicationCore.Render.Camera;
using OpenTK.Mathematics;

namespace ApplicationCore.UI; 

public sealed class UICamera : Camera {
    protected override void createView()       => View = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 10.0f), Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f));
    protected override void createProjection() => Projection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, 0.1f, 100.0f);

    public Vector2 screenToUISpace(Vector2 position) => new(position.X, RenderManager.WindowHeight - position.Y);
}