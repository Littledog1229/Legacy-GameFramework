using Engine.Render.Batch;
using Engine.Render.Texture;
using OpenTK.Mathematics;
using Prototype.World;

namespace Prototype.Tile; 

public class Tile {
    public string                  Identifier { get; init; } = null!;
    public TextureAtlas.SubTexture Texture    { get; init; }

    public Tile(string identifier, TextureAtlas.SubTexture texture) {
        Identifier = identifier;
        Texture    = texture;
    }

    public virtual void render(TileObject tile_object, ShapeBatch batch) {
        var world_pos = tile_object.Owner.Position.WorldPosition + tile_object.LocalPosition;
        var draw_pos = new Vector2(world_pos.X + 0.5f, world_pos.Y + 0.5f);
        
        batch.drawBox(Texture, draw_pos, Color4.White);
    }
}