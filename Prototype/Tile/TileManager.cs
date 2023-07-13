using Engine.Render.Texture;
using OpenTK.Mathematics;

namespace Prototype.Tile; 

public sealed class TileManager {
    private static TileManager Instance { get; set; } = null!;

    public static TextureAtlas TilesAtlas       { get; private set; } = null!;
    public static TextureAtlas TileOverlayAtlas { get; private set; } = null!;

    public static Tile getRegisteredTile(string identifier) => registered_tiles[identifier];

    
    private static readonly Dictionary<string, Tile> registered_tiles = new();

    public static void initialize() {
        if (Instance != null!)
            return;

        Instance = new TileManager();
        Instance.internalInitialize();
    }

    private void internalInitialize() {
        TilesAtlas       = new TextureAtlas("Resource.Texture.Tiles.png");
        TileOverlayAtlas = new TextureAtlas("Resource.Texture.TileOverlays.png");

        registerTiles();
    }

    private void registerTiles() {
        Vector2i size = new Vector2i(8, 8);

        registered_tiles["dirt"]  = new Tile("dirt",  TilesAtlas.create("dirt",  new Vector2i(1 , 1), size));
        registered_tiles["stone"] = new Tile("stone", TilesAtlas.create("stone", new Vector2i(11, 1), size));
    }
}