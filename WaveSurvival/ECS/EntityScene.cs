using ApplicationCore.Render;
using Engine;
using Engine.Scenes;

using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;
using EntityWorld  = Arch.Core.World;

namespace WaveSurvival.ECS; 

// Dont forget to use base.<function>() when overriding any update/render functions
public abstract class EntityScene : Scene {
    public EntityWorld  World        { get; } = EntityWorld.Create();
    public PhysicsWorld PhysicsWorld { get; } = new();

    public RenderPipeline ActivePipeline { get; private set; } = null!;

    protected List<IComponentQuery> UpdateQueries      { get; } = new();
    protected List<IComponentQuery> FixedUpdateQueries { get; } = new();
    protected List<IComponentQuery> LateUpdateQueries  { get; } = new();

    protected List<IComponentQuery> PreRenderQueries  { get; } = new();
    protected List<IComponentQuery> RenderQueries     { get; } = new();
    protected List<IComponentQuery> PostRenderQueries { get; } = new();

    protected abstract void   registerRender();
    protected abstract void deregisterRender();

    protected virtual void registerQueries() { }
    
    public override void start() {
        registerRender();
        registerQueries();
    }

    public override void destroy() {
        deregisterRender();
        
        World.Dispose();
    }

    public override void update() {
        foreach (var query in UpdateQueries)
            query.query(this);
    }
    public override void fixedUpdate() {
        PhysicsWorld.Step(Time.FixedDeltaTime);
        
        foreach (var query in FixedUpdateQueries)
            query.query(this);
    }
    public override void lateUpdate() {
        foreach (var query in LateUpdateQueries)
            query.query(this);
    }

    protected void render(RenderPipeline pipeline) {
        ActivePipeline = pipeline;
        
        foreach (var query in PreRenderQueries)
            query.query(this);
        foreach (var query in RenderQueries)
            query.query(this);
        foreach (var query in PostRenderQueries)
            query.query(this);

        ActivePipeline = null!;
    }
}