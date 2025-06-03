#version 460

in vec2 TextureCoordinates;

out vec4 OutColor;

uniform sampler2D image;

void main() {
    OutColor = texture(image, TextureCoordinates);
//    OutColor = vec4(TextureCoordinates, 1.0f, 1.0f);
}