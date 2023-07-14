using System.Reflection;
using Engine.Utility;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace ApplicationCore.Render.Texture; 

// TODO: Make an abstract Texture class that this inherits from
// TODO: Consider this as a "readonly" texture (where the data is constant)
// TODO: Make this texture class actually good, such as optional parameters to change texture properties
public sealed class Texture2D : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.Texture;
    
    public int Width  { get; set; }
    public int Height { get; set; }

    public void bindTexture(int slot) {
        GL.ActiveTexture((TextureUnit) ((int) TextureUnit.Texture0 + slot));
        bind();
    }

    public Texture2D(string filepath, Assembly? assembly = null) {
        createTexture(filepath, assembly);
        RenderManager.registerTexture2D(this);
    }
    
    public void bind()   => GL.BindTexture(TextureTarget.Texture2D, ObjectHandle);
    public void unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);

    public override void destroy() {
        GL.DeleteTexture(ObjectHandle);
        RenderManager.deregisterTexture2D(this);
    }

    private void createTexture(string filepath, Assembly? assembly = null) {
        ObjectHandle = GL.GenTexture();

        using var texture_stream = FileUtility.getEmbeddedStream(filepath, assembly);
        StbImage.stbi_set_flip_vertically_on_load(1);
        
        var image = ImageResult.FromStream(texture_stream, ColorComponents.RedGreenBlueAlpha);
        
        Width  = image.Width;
        Height = image.Height;
        
        bind();
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }
    
    internal void internalDestroy() => GL.DeleteTexture(ObjectHandle);
}