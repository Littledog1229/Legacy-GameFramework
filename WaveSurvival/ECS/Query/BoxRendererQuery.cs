using Arch.Core;
using OpenTK.Graphics.OpenGL;

namespace WaveSurvival.ECS.Query; 

public sealed class BoxRendererQuery : IComponentQuery {
    private readonly QueryDescription desc;

    public BoxRendererQuery() {
        desc = new QueryDescription();
        desc.WithAll<TransformComponent, BoxRenderer>();
    }
    
    public void query(EntityScene scene) {
        var singleplayer = scene as SingleplayerScene;
        
        var batch = singleplayer!.Batch;
        
        batch.begin(singleplayer.Camera);
        scene.World.Query(in desc, (ref TransformComponent transform, ref BoxRenderer renderer) => {
            batch.drawBox(transform.TruePosition, renderer.Color, transform.TrueRotation, renderer.Layer, transform.TrueScale);
        });
        batch.end();
    }
}