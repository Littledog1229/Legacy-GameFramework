using OpenTK.Mathematics;

namespace Sandbox3D.Mesh; 

// 3D world axis:
//  . X: East-West
//  . Y: Up-Down
//  . Z: North-South

public class CubeGenerator : MeshGenerator {
    private static readonly Vector3[] VERTEX_DATA = {
        new(0.0f, 0.0f, 0.0f), // 0
        new(0.0f, 1.0f, 0.0f), // 1
        new(1.0f, 1.0f, 0.0f), // 2
        new(1.0f, 0.0f, 0.0f), // 3

        new(1.0f, 0.0f, 1.0f), // 4
        new(1.0f, 1.0f, 1.0f), // 5
        new(0.0f, 1.0f, 1.0f), // 6
        new(0.0f, 0.0f, 1.0f), // 7
    };

    private static readonly uint[] INDEX_DATA = {
        0, 1, 2,
        0, 2, 3,
        
        4, 5, 6,
        4, 6, 7,
        
        8,  9, 10,
        8, 10, 11,
        
        12, 13, 14,
        12, 14, 15,
        
        16, 17, 18,
        16, 18, 19,
        
        20, 21, 22,
        20, 22, 23
    };
    
    public override (Vector3[], uint[]) generate(GenerationParameters? parameters = null) {
        var param = parameters ?? new GenerationParameters();
        var vertices = new Vector3[24];

        // North Face
        vertices[0] = (VERTEX_DATA[4] - param.Pivot) * param.Scale + param.Offset;
        vertices[1] = (VERTEX_DATA[5] - param.Pivot) * param.Scale + param.Offset;
        vertices[2] = (VERTEX_DATA[6] - param.Pivot) * param.Scale + param.Offset;
        vertices[3] = (VERTEX_DATA[7] - param.Pivot) * param.Scale + param.Offset;
        
        // East Face
        vertices[4] = (VERTEX_DATA[3] - param.Pivot) * param.Scale + param.Offset;
        vertices[5] = (VERTEX_DATA[2] - param.Pivot) * param.Scale + param.Offset;
        vertices[6] = (VERTEX_DATA[5] - param.Pivot) * param.Scale + param.Offset;
        vertices[7] = (VERTEX_DATA[4] - param.Pivot) * param.Scale + param.Offset;
        
        // South Face
        vertices[ 8] = (VERTEX_DATA[0] - param.Pivot) * param.Scale + param.Offset;
        vertices[ 9] = (VERTEX_DATA[1] - param.Pivot) * param.Scale + param.Offset;
        vertices[10] = (VERTEX_DATA[2] - param.Pivot) * param.Scale + param.Offset;
        vertices[11] = (VERTEX_DATA[3] - param.Pivot) * param.Scale + param.Offset;
        
        // West Face
        vertices[12] = (VERTEX_DATA[7] - param.Pivot) * param.Scale + param.Offset;
        vertices[13] = (VERTEX_DATA[6] - param.Pivot) * param.Scale + param.Offset;
        vertices[14] = (VERTEX_DATA[1] - param.Pivot) * param.Scale + param.Offset;
        vertices[15] = (VERTEX_DATA[0] - param.Pivot) * param.Scale + param.Offset;
        
        // Top Face
        vertices[16] = (VERTEX_DATA[1] - param.Pivot) * param.Scale + param.Offset;
        vertices[17] = (VERTEX_DATA[6] - param.Pivot) * param.Scale + param.Offset;
        vertices[18] = (VERTEX_DATA[5] - param.Pivot) * param.Scale + param.Offset;
        vertices[19] = (VERTEX_DATA[2] - param.Pivot) * param.Scale + param.Offset;
        
        // Bottom Face
        vertices[20] = (VERTEX_DATA[7] - param.Pivot) * param.Scale + param.Offset;
        vertices[21] = (VERTEX_DATA[0] - param.Pivot) * param.Scale + param.Offset;
        vertices[22] = (VERTEX_DATA[3] - param.Pivot) * param.Scale + param.Offset;
        vertices[23] = (VERTEX_DATA[4] - param.Pivot) * param.Scale + param.Offset;

        return (vertices, INDEX_DATA);
    }

    public override void generate(Vector3[] vertices, uint[] indices, GenerationParameters? parameters = null) {
        
    }
}