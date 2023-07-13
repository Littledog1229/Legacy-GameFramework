using Engine.Render;
using Engine.Render.Texture;
using OpenTK.Mathematics;

namespace SharpMinecraft.Block; 

public static class BlockAtlas {
    public const int ATLAS_SIZE_X = 16;
    public const int ATLAS_SIZE_Y = 16;
    
    public static Texture2D Texture { get; private set; } = null!;
    public static BlockUV[] UVs     { get; private set; } = null!;

    public static void init() {
        Texture = RenderManager.createTexture2D("Resource.Texture.terrain.png");
        UVs     = new BlockUV[ATLAS_SIZE_X * ATLAS_SIZE_Y];

        const float units_per_pixel_x = 1.0f / (ATLAS_SIZE_X * 16);
        const float units_per_pixel_y = 1.0f / (ATLAS_SIZE_Y * 16);
        var size = new Vector2(units_per_pixel_x * 16, units_per_pixel_y * 16);

        for (var y = 0; y < 16; y++) {
            for (var x = 0; x < 16; x++) {
                var min = new Vector2(x * 16 * units_per_pixel_x, (16 - y - 1) * 16 * units_per_pixel_y);
                
                UVs[y * 16 + x] = new BlockUV {
                    Min = min,
                    Max = min + size
                };
            }
        }
    }

    public static Vector2 getUV(int index, Vector2 model_uvs) => Vector2.Lerp(UVs[index].Min, UVs[index].Max, model_uvs);

    public struct BlockUV {
        public Vector2 Min { get; init; }
        public Vector2 Max { get; init; }

        public BlockUV() { }
    }
}