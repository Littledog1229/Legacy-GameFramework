#type vertex
#version 450 core

layout(location = 0) in vec3 vPosition;

uniform mat4 uProjection;
uniform mat4 uView;

void main() {
    gl_Position = uProjection * uView * vec4(vPosition.xyz, 1.0f);
}

#type fragment
#version 450 core

out vec4 oColor;

void main() {
    oColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);
}