namespace ApplicationCore.Render.Pipeline; 

public sealed class DefaultRenderPipeline : RenderPipeline {
    public Action<RenderPipeline>? OnRender { get; set; }

    protected internal override void initialize() {
        RenderStages.Add(renderDefaultStage);
    }

    private static void renderDefaultStage(RenderPipeline pipeline) {
        ((DefaultRenderPipeline) pipeline).OnRender?.Invoke(pipeline);
    }
}