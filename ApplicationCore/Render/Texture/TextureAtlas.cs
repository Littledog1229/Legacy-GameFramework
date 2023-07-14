using System.Reflection;
using OpenTK.Mathematics;

namespace ApplicationCore.Render.Texture; 

public sealed class TextureAtlas {
    public Texture2D Texture { get; }

    private readonly float normalized_pixel_x;
    private readonly float normalized_pixel_y;

    private readonly Dictionary<string, SubTexture> sub_textures = new();

    public TextureAtlas(string filepath, Assembly? assembly = null!) {
        Texture = new Texture2D(filepath, assembly);

        normalized_pixel_x = 1.0f / Texture.Width;
        normalized_pixel_y = 1.0f / Texture.Height;
    }

    public SubTexture create(string identifier, Vector2i position, Vector2i size) {
        Vector2 pixel_size = new(size.X     * normalized_pixel_x, size.Y     * normalized_pixel_y);
        Vector2 pixel_pos  = new(position.X * normalized_pixel_x, position.Y * normalized_pixel_y);
        
        Vector2[] uvs = {
            new(pixel_pos.X,                  1.0f - (pixel_pos.Y + pixel_size.Y)),
            new(pixel_pos.X,                  1.0f -  pixel_pos.Y),
            new(pixel_pos.X + pixel_size.X, 1.0f -  pixel_pos.Y),
            new(pixel_pos.X + pixel_size.X, 1.0f - (pixel_pos.Y + pixel_size.Y))
        };
        
        var sub_texture = new SubTexture {
            Owner      = this,
            Identifier = identifier,
            Position   = position,
            Size       = size,
            UVs        = uvs
        };
        sub_textures.Add(identifier, sub_texture);

        return sub_texture;
    }
    public SubTexture get   (string identifier) => sub_textures[identifier];

    public struct SubTexture {
        public TextureAtlas Owner      { get; init; }
        public string       Identifier { get; init; }
        public Vector2i     Position   { get; init; }
        public Vector2i     Size       { get; init; }
        public Vector2[]    UVs        { get; init; }
    }
}