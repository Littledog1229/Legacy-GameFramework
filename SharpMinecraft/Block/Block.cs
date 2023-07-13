namespace SharpMinecraft.Block; 

public class Block {
    private readonly BlockSettings settings;

    public string     Name { get; private set; }
    public BlockModel Model       => settings.Model;
    public bool       Transparent => settings.Transparent;
    public uint[]     Textures    => settings.Textures;

    public Block(string name, BlockSettings settings) {
        Name = name;
        this.settings = settings;
    }
    
    public struct BlockSettings {
        public BlockModel Model       { get; private set; }
        public bool       Transparent { get; private set; }
        public uint[]     Textures    { get; private set; }

        public BlockSettings() {
            Model       = BlockModel.DEFAULT;
            Transparent = false;
            Textures    = new uint[] { 0 };
        }

        public BlockSettings transparent(bool value)   { Transparent = value;    return this; }
        public BlockSettings model(BlockModel model)   { Model       = model;    return this; }
        public BlockSettings textures(uint[] textures) { Textures    = textures; return this; }
    }
}