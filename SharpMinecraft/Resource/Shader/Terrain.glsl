#type vertex
#version 450 core

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec4 vColor;
layout(location = 2) in vec2 vUV;

layout(location = 0) out vec4 fColor;
layout(location = 1) out vec2 fUV;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position = uProjection * uView * vec4(vPosition.xyz, 1.0);
    fColor      = vColor;
    fUV         = vUV;
}

#type fragment
#version 450 core

layout(location = 0) in vec4 fColor;
layout(location = 1) in vec2 fUV;

out vec4 oColor;

uniform sampler2D uTexture;

void main() {
    oColor = texture(uTexture, fUV) * fColor;
    
    if (oColor.a < 0.1f)
        discard;
}