#type vertex
#version 450

layout (location = 0) in vec4 Position;
layout (location = 1) in uint  Index;

layout (location = 0) flat out uint fIndex;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position = uProjection * uView * vec4(Position.xy, 0, 1);
    fIndex      = Index;
}

#type fragment
#version 450

precision highp int;

layout (location = 0) flat in uint fIndex;

layout (location = 0) out uvec3 oColor;

void main() {
    oColor = uvec3(fIndex, 0, gl_PrimitiveID);
}