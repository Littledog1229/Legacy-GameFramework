namespace SharpMinecraft.Block; 

public static class BlockRegistry {
    private static readonly List<Block> registered_blocks = new() {
        new Block("Air",          new Block.BlockSettings().model(BlockModel.TRANSPARENT).transparent(true)),
        new Block("Bedrock",      new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 0  })),
        new Block("Stone",        new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 1  })),
        new Block("Dirt",         new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 2  })),
        new Block("Grass",        new Block.BlockSettings().model(BlockModel.GRASS)  .textures(new uint[] { 7, 6, 2 })),
        new Block("Wooden Planks",new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 76 })),
        new Block("Cobblestone",  new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 16 })),
        new Block("Sand",         new Block.BlockSettings().model(BlockModel.DEFAULT).textures(new uint[] { 3 }))
    };

    public static Block getBlock(int index) => registered_blocks[index];
}