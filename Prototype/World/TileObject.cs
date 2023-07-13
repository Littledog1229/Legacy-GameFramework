using OpenTK.Mathematics;

namespace Prototype.World; 

public struct TileObject {
    public Chunk    Owner         { get; init; }
    public uint     TileIndex     { get; set;  }
    public Vector2i LocalPosition { get; init; }

    public TileObject(Chunk owner, uint tile_index, Vector2i pos) {
        Owner         = owner;
        TileIndex     = tile_index;
        LocalPosition = pos;
    }
}