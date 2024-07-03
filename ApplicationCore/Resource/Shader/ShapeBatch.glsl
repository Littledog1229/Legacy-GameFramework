#type vertex
#version 450

layout (location = 0) in vec4  Position;
layout (location = 1) in vec4  Color;
layout (location = 2) in vec2  UV;
layout (location = 3) in float TextureCoord;

layout (location = 0) out vec4  fColor;
layout (location = 1) out vec2  fUV;
layout (location = 2) out float fTextureCoord;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position   = uProjection * uView * vec4(Position.xyz, 1);
    fColor        = Color;
    fUV           = UV;
    fTextureCoord = TextureCoord;
}

#type fragment
#version 450

layout (location = 0) in vec4  fColor;
layout (location = 1) in vec2  fUV;
layout (location = 2) in float fTextureCoord;

layout (location = 0) out vec4 oColor;

uniform sampler2D[8] uTextures;

void main() {
    if (fTextureCoord == -1) {
        oColor = fColor;
        return;
    }

    oColor = texture(uTextures[int(fTextureCoord)], fUV) * fColor;
    
    if (oColor.a <= 0.1f)
        discard;
}