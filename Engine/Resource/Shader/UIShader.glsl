#type vertex
#version 450

layout (location = 0) in vec2  Position;
layout (location = 1) in vec4  Color;

layout (location = 0) out vec4  fColor;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position   = uProjection * uView * vec4(Position, 0, 1);
    fColor        = Color;
}

#type fragment
#version 450

layout (location = 0) in vec4  fColor;

layout (location = 0) out vec4 oColor;

void main() {
    oColor = fColor;
}