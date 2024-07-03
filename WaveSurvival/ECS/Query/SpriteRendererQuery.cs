using Arch.Core;
using OpenTK.Graphics.OpenGL;

namespace WaveSurvival.ECS.Query; 

public sealed class SpriteRendererQuery : IComponentQuery {
    private readonly QueryDescription desc;

    public SpriteRendererQuery() {
        desc = new QueryDescription();
        desc.WithAll<TransformComponent, SpriteRenderer>();
    }
    
    public void query(EntityScene scene) {
        var singleplayer = scene as SingleplayerScene;
        
        var batch = singleplayer!.Batch;
        
        batch.begin(singleplayer.Camera);
        scene.World.Query(in desc, (ref TransformComponent transform, ref SpriteRenderer renderer) => {
            batch.drawBox(transform.TruePosition, renderer.Tint, transform.TrueRotation, renderer.Layer, transform.TrueScale, renderer.Sprite);
        });
        batch.end();
    }
}