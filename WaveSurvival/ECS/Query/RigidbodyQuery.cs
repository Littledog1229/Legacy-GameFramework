using Arch.Core;
using Engine.Physics;
using Engine.Utility;

namespace WaveSurvival.ECS.Query; 

public sealed class RigidbodyQuery : IComponentQuery {
    private readonly QueryDescription desc = new();

    public RigidbodyQuery() {
        desc.WithAll<TransformComponent, Rigidbody>();
    }
    
    public void query(EntityScene scene) {
        scene.World.Query(in desc, (ref TransformComponent transform, ref Rigidbody rigidbody) => {
            transform.TruePosition = rigidbody.Body.Position.toVector2();
            transform.TrueRotation = rigidbody.Body.Rotation.toDegrees();
        });
    }
}