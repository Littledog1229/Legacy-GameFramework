namespace Engine.Scenes; 

public sealed class SceneManager : EngineSystem {
    private static SceneManager Instance { get; set; } = null!;

    private readonly List<Scene> active_scenes = new();
    private bool started;
    
    public static SceneManager create() {
        if (Instance != null!)
            return Instance;

        Instance = new SceneManager();
        return Instance;
    }

    public static void addScene(Scene scene, SceneAddMode mode = SceneAddMode.Single) {
        if (mode == SceneAddMode.Single) {
            foreach (var active_scene in Instance.active_scenes) {
                active_scene.unload();
                active_scene.destroy();
            }
            
            Instance.active_scenes.Clear();
        }
        
        Instance.active_scenes.Add(scene);
        if (Instance.started)
            scene.start();
    }

    public static bool removeScene(Scene scene) {
        if (!Instance.active_scenes.Contains(scene))
            return true;
        
        scene.unload();
        scene.destroy();

        return Instance.active_scenes.Remove(scene);
    }

    public enum SceneAddMode {
        Additive,
        Single
    }
    
    protected internal override void start() {
        foreach (var scene in active_scenes)
            scene.start();

        started = true;
    }
    protected internal override void unload() {
        foreach (var scene in active_scenes)
            scene.unload();
    }
    protected internal override void destroy() {
        foreach (var scene in active_scenes)
            scene.destroy();
        
        active_scenes.Clear();
    }

    protected internal override void fixedUpdate() {
        foreach (var scene in active_scenes)
            scene.fixedUpdate();
    }
    protected internal override void update() {
        foreach (var scene in active_scenes)
            scene.update();
    }
    protected internal override void lateUpdate() {
        foreach (var scene in active_scenes)
            scene.lateUpdate();
    }

    /*protected internal override void preRender() {
        foreach (var scene in active_scenes)
            scene.preRender();
    }
    protected internal override void render() {
        foreach (var scene in active_scenes)
            scene.render();
    }
    protected internal override void postRender() {
        foreach (var scene in active_scenes)
            scene.postRender();
    }*/
}