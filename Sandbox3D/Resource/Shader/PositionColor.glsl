#type vertex
#version 450 core

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec4 vColor;

layout(location = 0) out vec4 fColor;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position = uProjection * uView * vec4(vPosition.xyz, 1.0f);
    fColor      = vColor;
}

#type fragment
#version 450 core

layout(location = 0) in vec4 fColor;

out vec4 oColor;

void main() {
    oColor = fColor;
}