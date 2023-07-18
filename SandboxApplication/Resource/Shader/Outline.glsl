#type vertex
#version 450

layout (location = 0) in vec4 Position;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position = uProjection * uView * vec4(Position.xy, 0, 1);
}

#type fragment
#version 450

out vec4 oColor;

uniform vec4 uOutlineColor;

void main() {
    oColor = uOutlineColor;//vec4(1.0f, 0.0f, 1.0f, 1.0f);
}